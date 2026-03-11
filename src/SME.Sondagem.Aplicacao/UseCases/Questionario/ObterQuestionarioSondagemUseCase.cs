using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ObterQuestionarioSondagemUseCase : IObterQuestionarioSondagemUseCase
{
    private readonly RepositoriosElastic _repositoriosElastic;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly IAlunoPapService _alunoPapService;
    private readonly IControleAcessoService _controleAcessoService;

    public ObterQuestionarioSondagemUseCase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        IAlunoPapService alunoPapService,
        IControleAcessoService controleAcessoService)
    {
        _repositoriosElastic = repositoriosElastic ?? throw new ArgumentNullException(nameof(repositoriosElastic));
        _repositoriosSondagem = repositoriosSondagem ?? throw new ArgumentNullException(nameof(repositoriosSondagem));
        _alunoPapService = alunoPapService ?? throw new ArgumentNullException(nameof(alunoPapService));
        _controleAcessoService = controleAcessoService ?? throw new ArgumentNullException(nameof(controleAcessoService));
    }

    public async Task<QuestionarioSondagemDto> ObterQuestionarioSondagem([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        var turma = await ValidarFiltroEModalidade(filtro, cancellationToken);

        var sondagemAtiva = await ObterSondagemAtivaOuLancarExcecao(cancellationToken);

        return await ProcessarQuestionario(filtro, turma, sondagemAtiva, cancellationToken);
    }

    private async Task<TurmaElasticDto> ValidarFiltroEModalidade(FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        var turma = await _repositoriosElastic.RepositorioElasticTurma.ObterTurmaPorId(filtro, cancellationToken)
            ?? throw new RegraNegocioException("Turma não localizada", 400);

        if (filtro.ProficienciaId == 0)
            throw new RegraNegocioException("A proficiência é obrigatória no filtro", 400);

        return turma;
    }

    private async Task<Dominio.Entidades.Sondagem.Sondagem> ObterSondagemAtivaOuLancarExcecao(CancellationToken cancellationToken)
    {
        return await _repositoriosSondagem.RepositorioSondagem.ObterSondagemAtiva(cancellationToken)
            ?? throw new RegraNegocioException("Não há sondagem ativa cadastrada no sistema");
    }

    private async Task<QuestionarioSondagemDto> ProcessarQuestionario(
        FiltroQuestionario filtro,
        TurmaElasticDto turma,
        Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
        CancellationToken cancellationToken)
    {
        var modalidade = turma.Modalidade;
        int anoLetivo = filtro.AnoLetivo != 0 ? filtro.AnoLetivo : turma.AnoLetivo;

        if (!int.TryParse(turma.AnoTurma, out int ano))
            throw new RegraNegocioException("Ano da turma inválido");

        ValidarModalidadeEAno(modalidade, ano);

        var questoesAtivas = await ObterQuestoesAtivasOuLancarExcecao(modalidade, ano, turma.AnoLetivo, filtro.ProficienciaId, cancellationToken);

        var contextoProcessamento = await ConstruirContextoProcessamento(filtro, turma, sondagemAtiva, cancellationToken);

        var dadosAlunos = await ObterDadosAlunos(filtro.TurmaId, turma.AnoLetivo, contextoProcessamento, cancellationToken);

        var linguaPortuguesaSegundaLingua = contextoProcessamento.QuestaoLinguaPortuguesa;

        var questoesIds = ObterQuestoesIdsPorTipo(questoesAtivas);

        var alunos = await ObterAlunosOuLancarExcecao(filtro.TurmaId, anoLetivo, cancellationToken);     

        var codigosAlunos = alunos.Select(a => a.CodigoAluno).ToList();

        var respostasAlunosPorQuestoes = await _repositoriosSondagem.RepositorioRespostaAluno.ObterRespostasAlunosPorQuestoesAsync(
            codigosAlunos.Select(x => (long)x).ToList(),
            questoesIds.Select(x => (long)x).ToList(),
            sondagemAtiva.Id,
            cancellationToken);

        var respostas = respostasAlunosPorQuestoes.Values;

        var criadoMaisAntigo = respostas
            .OrderBy(r => r.CriadoEm)
            .FirstOrDefault();

        var alteradoMaisRecente = respostas
            .Where(r => r.AlteradoEm.HasValue)
            .OrderByDescending(r => r.AlteradoEm)
            .FirstOrDefault();

        var nomeInseridoPor = criadoMaisAntigo is not null
            ? $"Inserido por {criadoMaisAntigo.CriadoPor} ({criadoMaisAntigo.CriadoRF}) em {criadoMaisAntigo.CriadoEm:dd/MM/yyyy HH:mm}"
            : null;

        var nomeAlteradoPor = alteradoMaisRecente is not null
            ? $"Alterado por {alteradoMaisRecente.AlteradoPor} ({alteradoMaisRecente.AlteradoRF}) em {alteradoMaisRecente.AlteradoEm:dd/MM/yyyy HH:mm}"
            : null;       

        var respostasProcessadas = ProcessarRespostas(
           contextoProcessamento.RespostasAlunosPorQuestoes,
           linguaPortuguesaSegundaLingua!,
           contextoProcessamento.Alunos,
           sondagemAtiva.PeriodosBimestre.OrderBy(c => c.DataInicio).FirstOrDefault()!.DataInicio
       );

        var estudantes = await ConstruirEstudantes(dadosAlunos, sondagemAtiva, contextoProcessamento, respostasProcessadas);

        var questaoId = questoesAtivas.FirstOrDefault(x => x.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua)?.Id ?? 0;

        var tituloTabelaRespostas = ObterTituloTabelaRespostas(questoesAtivas);

        return new QuestionarioSondagemDto
        {
            QuestaoId = questaoId,
            SondagemId = sondagemAtiva.Id,
            TituloTabelaRespostas = tituloTabelaRespostas,
            PodeSalvar = await _controleAcessoService.ValidarPermissaoAcessoAsync(turma.CodigoTurma.ToString(), cancellationToken),
            Estudantes = estudantes,
            InseridoPor = nomeInseridoPor,
            AlteradoPor = nomeAlteradoPor
        };
    }
    protected virtual async Task<DadosAlunosDto> ObterDadosAlunos(
        int turmaId,
        int anoLetivo,
        ContextoProcessamentoDto contexto,
        CancellationToken cancellationToken)
    {
        var codigosAlunos = contexto.CodigosAlunos;

        var alunosComPap = await _alunoPapService.VerificarAlunosPossuemProgramaPapAsync(
            codigosAlunos,
            anoLetivo,
            cancellationToken);

        var questaoLinguaPortuguesa = contexto.QuestaoLinguaPortuguesa;

        var alunosComLinguaPortuguesaSegundaLingua = await _repositoriosSondagem.RepositorioRespostaAluno
            .VerificarAlunosPossuiLinguaPortuguesaAsync(
                codigosAlunos,
                questaoLinguaPortuguesa,
                cancellationToken);

        return new DadosAlunosDto
        {
            AlunosComPap = alunosComPap,
            AlunosComLinguaPortuguesaSegundaLingua = alunosComLinguaPortuguesaSegundaLingua
        };
    }
    private static void ValidarModalidadeEAno(int modalidade, int ano)
    {
        if (modalidade != (int)Modalidade.Fundamental && modalidade != (int)Modalidade.EJA)
            throw new ErroNaoEncontradoException("Não há questionário para a modalidade informada");

        if (ano is < 1 or > 3)
            throw new ErroNaoEncontradoException("Não há questionário para a série informada");
    }

    private async Task<IEnumerable<Dominio.Entidades.Questionario.Questao>> ObterQuestoesAtivasOuLancarExcecao(
        int modalidade,
        int ano,
        int anoLetivo,
        int proficienciaId,
        CancellationToken cancellationToken)
    {
        var questoesAtivas = await _repositoriosSondagem.RepositorioQuestao.ObterQuestoesAtivasPorFiltroAsync(
            modalidade,
            anoLetivo,
            proficienciaId,
            ano,
            cancellationToken);

        return questoesAtivas != null && questoesAtivas.Any()
            ? questoesAtivas
            : throw new ErroNaoEncontradoException("Não há questões ativas para o questionário com os filtros informados");
    }

    private static List<int> ObterQuestoesIdsPorTipo(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        return questoesAtivas
            .Select(q => q.Id)
            .ToList();
    }

    private static int ObterIdQuestionario(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        var primeiraQuestao = questoesAtivas.FirstOrDefault();
        if (primeiraQuestao == null)
            return 0;
        return primeiraQuestao.QuestionarioId;
    }

    private async Task<IEnumerable<AlunoElasticDto>> ObterAlunosOuLancarExcecao(int turmaId, int anoLetivo, CancellationToken cancellationToken)
    {
        var alunos = await _repositoriosElastic.RepositorioElasticAluno.ObterAlunosPorIdTurma(turmaId, anoLetivo, cancellationToken);

        return alunos != null && alunos.Any()
            ? alunos
            : throw new ErroNaoEncontradoException("Não há alunos cadastrados para a turma informada");
    }

    private static Task<List<ColunaQuestionarioDto>> ObterColunasOuLancarExcecao(
        ICollection<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> periodosBimestre,
        IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas,
        int? bimestreId)
    {
        var possuiSubperguntas = PossuiQuestaoVinculo(questoesAtivas);

        if (possuiSubperguntas)
        {
            if (!bimestreId.HasValue)
                throw new RegraNegocioException(MensagemNegocioComuns.BIMESTRE_OBRIGATORIO);

            var periodo = periodosBimestre
                .FirstOrDefault(p => !p.Excluido && p.BimestreId == bimestreId.Value)
                ?? throw new ErroNaoEncontradoException("Período do bimestre não encontrado.");

            var agora = DateTime.Now;
            var periodoAtivo = agora >= periodo.DataInicio && agora <= periodo.DataFim;

            var colunasPorQuestao = questoesAtivas
                .Where(q => !q.Excluido && q.Tipo != TipoQuestao.QuestaoComSubpergunta)
                .OrderBy(q => q.Ordem)
                .Select(q => new ColunaQuestionarioDto
                {
                    IdCiclo = bimestreId.Value,
                    DescricaoColuna = q.Nome,
                    PeriodoBimestreAtivo = periodoAtivo,
                    QuestaoSubrespostaId = q.Id,
                    OpcaoResposta = ObterOpcoesRespostasPorQuestao(q.Id, questoesAtivas)
                })
                .ToList();
            return colunasPorQuestao.Count != 0
                ? Task.FromResult(colunasPorQuestao)
                : throw new ErroNaoEncontradoException(
                    "Não foi possível obter as colunas das subperguntas");
        }

        var questaoBimestre = questoesAtivas.FirstOrDefault(q => q.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua);
        var opcoesResposta = ObterOpcoesRespostasPorQuestao(questaoBimestre!.Id, questoesAtivas);

        var bimestresAtivos = periodosBimestre
            .Where(p => !p.Excluido)
            .Select(p => new ColunaQuestionarioDto
            {
                IdCiclo = p.BimestreId,
                DescricaoColuna = p.Bimestre?.Descricao ?? string.Empty,
                PeriodoBimestreAtivo = DateTime.Now >= p.DataInicio && DateTime.Now <= p.DataFim,
                OpcaoResposta = opcoesResposta
            })
            .ToList();

        return bimestresAtivos.Count != 0
            ? Task.FromResult(bimestresAtivos)
            : throw new ErroNaoEncontradoException("Não foi possível obter as colunas dos ciclos");
    }

    private static List<OpcaoRespostaDto> ObterOpcoesRespostasPorQuestao(int questaoId, IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        var retorno = questoesAtivas
            .Where(q => q.Id == questaoId)
            .SelectMany(q => q.QuestaoOpcoes)
            .OrderBy(qo => qo.Ordem)
            .Select(qo => new OpcaoRespostaDto
            {
                Id = qo.OpcaoRespostaId,
                Ordem = qo.Ordem,
                DescricaoOpcaoResposta = qo.OpcaoResposta.DescricaoOpcaoResposta,
                Legenda = qo.OpcaoResposta.Legenda,
                CorFundo = qo.OpcaoResposta.CorFundo,
                CorTexto = qo.OpcaoResposta.CorTexto
            })
            .ToList();

        return retorno;
    }

    private static bool PossuiQuestaoVinculo(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        return questoesAtivas.Any(q => q.QuestaoVinculo?.Tipo == TipoQuestao.QuestaoComSubpergunta);
    }

    private async Task<List<EstudanteQuestionarioDto>> ConstruirEstudantes(
         DadosAlunosDto dadosAlunos,
         Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
         ContextoProcessamentoDto contexto,
         RespostasProcessadasDto respostasProcessadas)
    {
        var estudantes = new List<EstudanteQuestionarioDto>();

        foreach (var aluno in contexto.Alunos)
        {
            var colunasAluno = contexto.Colunas
                .Select(c => ConstruirColunaAluno(c, aluno, sondagemAtiva, contexto.QuestaoIdPrincipal, respostasProcessadas.RespostasConvertidas))
                .ToList();

            var estudante = await ConstruirEstudante(aluno, dadosAlunos, colunasAluno, aluno.CodigoAluno);
            estudantes.Add(estudante);
        }

        return [.. estudantes.OrderBy(e => e.Nome)];
    }

    protected virtual Task<EstudanteQuestionarioDto> ConstruirEstudante(
        AlunoElasticDto aluno,
        DadosAlunosDto dadosAlunos,
        List<ColunaQuestionarioDto> colunasAluno,
        int codigoAluno)
    {
        var estudante = new EstudanteQuestionarioDto
        {
            NumeroAlunoChamada = aluno.NumeroAlunoChamada,
            Codigo = codigoAluno,
            Nome = aluno.NomeAluno ?? string.Empty,
            LinguaPortuguesaSegundaLingua = dadosAlunos.AlunosComLinguaPortuguesaSegundaLingua.TryGetValue(codigoAluno, out var lingua) && lingua,
            Pap = dadosAlunos.AlunosComPap.TryGetValue(codigoAluno, out var pap) && pap,
            PossuiDeficiencia = aluno.PossuiDeficiencia == 1,
            Coluna = colunasAluno
        };

        return Task.FromResult(estudante);
    }
    protected static ColunaQuestionarioDto ConstruirColunaAluno(
         ColunaQuestionarioDto colunaBase,
         AlunoElasticDto aluno,
         Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
         long questaoIdPrincipal,
         Dictionary<(int CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> respostasAlunosPorQuestoes,
         bool ehRelatorio = false)
    {
        long questaoIdChave = colunaBase.QuestaoSubrespostaId ?? (int)questaoIdPrincipal;
        int? bimestreIdChave = colunaBase.IdCiclo == 0 ? null : colunaBase.IdCiclo;

        int codigoAluno = aluno.CodigoAluno;
        int situacaoMatricula = aluno.CodigoSituacaoMatricula;
        DateTime dataSituacaoMatricula = aluno.DataSituacao;

        var chave = (CodigoAluno: codigoAluno, BimestreId: bimestreIdChave, QuestaoId: questaoIdChave);
        var possuiResposta = respostasAlunosPorQuestoes.TryGetValue(chave, out var resposta);

        var podeLancarSondagem = sondagemAtiva.PeriodosBimestre.Any(p => dataSituacaoMatricula <= p.DataInicio)
            && situacaoMatricula == (int)SituacaoMatriculaAluno.Ativo;

        return new ColunaQuestionarioDto
        {
            IdCiclo = colunaBase.IdCiclo,
            DescricaoColuna = colunaBase.DescricaoColuna,
            PeriodoBimestreAtivo = podeLancarSondagem && colunaBase.PeriodoBimestreAtivo,
            QuestaoSubrespostaId = colunaBase.QuestaoSubrespostaId,
            OpcaoResposta = ehRelatorio
                ? colunaBase.OpcaoResposta?.Where(op => op.Id == resposta?.OpcaoRespostaId)
                : colunaBase.OpcaoResposta,
            Resposta = ConstruirResposta(possuiResposta, resposta)
        };
    }
    private static RespostaDto? ConstruirResposta(bool possuiResposta, RespostaAluno? resposta)
    {
        if (!possuiResposta || resposta is null)
            return null;

        var opcaoRespostaId = resposta.OpcaoRespostaId == 0 ? null : resposta.OpcaoRespostaId;

        return new RespostaDto
        {
            Id = resposta.Id,
            OpcaoRespostaId = opcaoRespostaId
        };
    }

    private static string ObterTituloTabelaRespostas(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        var primeiraQuestao = questoesAtivas.FirstOrDefault();

        return PossuiQuestaoVinculo(questoesAtivas)
            ? questoesAtivas.FirstOrDefault(q => q.QuestaoVinculo?.Tipo == TipoQuestao.QuestaoComSubpergunta)?.QuestaoVinculo?.Nome!
            : primeiraQuestao?.Nome ?? string.Empty;
    }

    private async Task<ContextoProcessamentoDto> ConstruirContextoProcessamento(
      FiltroQuestionario filtro,
      TurmaElasticDto turma,
      Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
      CancellationToken cancellationToken)
    {
        var modalidade = turma.Modalidade;
        int ano = int.TryParse(turma.AnoTurma, out int anoTurma) ? anoTurma : filtro.Ano;
        int anoLetivo = filtro.AnoLetivo != 0 ? filtro.AnoLetivo : turma.AnoLetivo;

        ValidarModalidadeEAno(modalidade, ano);

        var questoesAtivas = await ObterQuestoesAtivasOuLancarExcecao(modalidade, ano, anoLetivo, filtro.ProficienciaId, cancellationToken);

        var questaoLinguaPortuguesa = questoesAtivas.FirstOrDefault(x => x.Tipo == TipoQuestao.LinguaPortuguesaSegundaLingua);
        var questoesIds = ObterQuestoesIdsPorTipo(questoesAtivas);

        var bimestresForaDoPadrao = await _repositoriosSondagem.RepositorioBimestre
            .ObterBimestresPorQuestionarioIdAsync(ObterIdQuestionario(questoesAtivas), cancellationToken);

        var periodosBimestre = bimestresForaDoPadrao?.Count > 0 ? bimestresForaDoPadrao : sondagemAtiva.PeriodosBimestre;

        var colunas = await ObterColunasOuLancarExcecao(
            filtro.BimestreId.HasValue
                ? periodosBimestre.Where(p => p.BimestreId == filtro.BimestreId.Value).ToList()
                : periodosBimestre,
            questoesAtivas,
            filtro.BimestreId
        );

        var alunos = await ObterAlunosOuLancarExcecao(filtro.TurmaId, anoLetivo, cancellationToken);

        var codigosAlunos = alunos.Select(a => a.CodigoAluno).ToList();

        var respostasAlunosPorQuestoes = await _repositoriosSondagem.RepositorioRespostaAluno.ObterRespostasAlunosPorQuestoesAsync(
            codigosAlunos.Select(x => (long)x).ToList(),
            questoesIds.Select(x => (long)x).ToList(),
            sondagemAtiva.Id,
            cancellationToken);

        var questaoIdPrincipal = questoesAtivas.First(x => x.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua).Id;

        return new ContextoProcessamentoDto
        {
            QuestoesAtivas = questoesAtivas,
            QuestaoLinguaPortuguesa = questaoLinguaPortuguesa,
            Colunas = colunas,
            Alunos = alunos,
            CodigosAlunos = codigosAlunos,
            RespostasAlunosPorQuestoes = respostasAlunosPorQuestoes.ToDictionary(
                x => (x.Key.CodigoAluno, x.Key.BimestreId, x.Key.QuestaoId),
                x => x.Value
            ),
            QuestaoIdPrincipal = questaoIdPrincipal
        };
    }

    protected static RespostasProcessadasDto ProcessarRespostas(
        Dictionary<(long CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> respostasAlunosPorQuestoes,
        Dominio.Entidades.Questionario.Questao linguaPortuguesaSegundaLingua,
        IEnumerable<AlunoElasticDto> alunosAtivos,
        DateTime dataInicioSondagem)
    {
        var codigosAlunosAtivos = alunosAtivos
            .Where(a => a.DataSituacao <= dataInicioSondagem)
            .Select(a => a.CodigoAluno)
            .ToHashSet();

        var respostasAuditoria = respostasAlunosPorQuestoes.Values
            .Where(r => r.AlunoId.HasValue && codigosAlunosAtivos.Contains(r.AlunoId.Value))
            .ToList();

        if (linguaPortuguesaSegundaLingua != null)
            respostasAuditoria = respostasAuditoria
                .Where(x => x.QuestaoId != linguaPortuguesaSegundaLingua.Id)
                .ToList();

        var criadoMaisAntigo = respostasAuditoria
            .OrderBy(r => r.CriadoEm)
            .FirstOrDefault();

        var alteradoMaisRecente = respostasAuditoria
            .Where(r => r.AlteradoEm.HasValue)
            .OrderByDescending(r => r.AlteradoEm)
            .FirstOrDefault();

        var nomeInseridoPor = criadoMaisAntigo is not null
            ? $"Inserido por {criadoMaisAntigo.CriadoPor} ({criadoMaisAntigo.CriadoRF}) em {criadoMaisAntigo.CriadoEm:dd/MM/yyyy HH:mm}"
            : null;

        var nomeAlteradoPor = alteradoMaisRecente is not null
            ? $"Alterado por {alteradoMaisRecente.AlteradoPor} ({alteradoMaisRecente.AlteradoRF}) em {alteradoMaisRecente.AlteradoEm:dd/MM/yyyy HH:mm}"
            : null;

        var respostasConvertidas = respostasAlunosPorQuestoes
            .Where(x => x.Value?.OpcaoRespostaId is not null && codigosAlunosAtivos.Contains((int)x.Key.CodigoAluno))
            .GroupBy(x => (
                CodigoAluno: (int)x.Key.CodigoAluno,
                x.Key.BimestreId,
                x.Key.QuestaoId
            ))
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Value!).First()
            );

        return new RespostasProcessadasDto
        {
            InseridoPor = nomeInseridoPor,
            AlteradoPor = nomeAlteradoPor,
            RespostasConvertidas = respostasConvertidas
        };
    }

}