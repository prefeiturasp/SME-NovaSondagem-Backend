using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ObterQuestionarioSondagemUseCase : IObterQuestionarioSondagemUseCase
{
    private readonly IRepositorioElasticTurma _repositorioElasticTurma;
    private readonly IRepositorioElasticAluno _repositorioElasticAluno;
    private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;
    private readonly IRepositorioSondagem _repositorioSondagem;
    private readonly IRepositorioQuestao _repositorioQuestao;
    private readonly IAlunoPapService _alunoPapService;

    public ObterQuestionarioSondagemUseCase(
        IRepositorioElasticTurma repositorioElasticTurma,
        IRepositorioElasticAluno repositorioElasticAluno,
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IAlunoPapService alunoPapService,
        IRepositorioSondagem repositorioSondagem,
        IRepositorioQuestao repositorioQuestao)
    {
        _repositorioElasticTurma = repositorioElasticTurma ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
        _repositorioElasticAluno = repositorioElasticAluno ?? throw new ArgumentNullException(nameof(repositorioElasticAluno));
        _repositorioRespostaAluno = repositorioRespostaAluno ?? throw new ArgumentNullException(nameof(repositorioRespostaAluno));
        _alunoPapService = alunoPapService ?? throw new ArgumentNullException(nameof(alunoPapService));
        _repositorioSondagem = repositorioSondagem ?? throw new ArgumentNullException(nameof(repositorioSondagem));
        _repositorioQuestao = repositorioQuestao ?? throw new ArgumentNullException(nameof(repositorioQuestao));
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
        var turma = await _repositorioElasticTurma.ObterTurmaPorId(filtro, cancellationToken)
            ?? throw new ErroNaoEncontradoException("Turma não localizada");

        if (filtro.ProficienciaId == 0)
            throw new RegraNegocioException("A proficiência é obrigatória no filtro");

        return turma;
    }

    private async Task<Dominio.Entidades.Sondagem.Sondagem> ObterSondagemAtivaOuLancarExcecao(CancellationToken cancellationToken)
    {
        return await _repositorioSondagem.ObterSondagemAtiva(cancellationToken)
            ?? throw new ErroInternoException("Não há sondagem ativa cadastrada no sistema");
    }

    private async Task<QuestionarioSondagemDto> ProcessarQuestionario(
        FiltroQuestionario filtro,
        TurmaElasticDto turma,
        Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
        CancellationToken cancellationToken)
    {
        var modalidade = turma.Modalidade;

        if (!int.TryParse(turma.AnoTurma, out int ano))
            throw new ErroInternoException("Ano da turma inválido");

        ValidarModalidadeEAno(modalidade, ano);

        var questoesAtivas = await ObterQuestoesAtivasOuLancarExcecao(modalidade, ano, turma.AnoLetivo, filtro.ProficienciaId, cancellationToken);

        var questaoLinguaPortuguesa = questoesAtivas.FirstOrDefault(x => x.Tipo == TipoQuestao.LinguaPortuguesaSegundaLingua);

        var questoesIds = ObterQuestoesIdsPorTipo(questoesAtivas);

        var alunos = await ObterAlunosOuLancarExcecao(filtro.TurmaId, cancellationToken);

        var colunas = await ObterColunasOuLancarExcecao(sondagemAtiva.PeriodosBimestre, questoesAtivas, filtro.BimestreId);

        var codigosAlunos = alunos.Select(a => (int)a.CodigoAluno).ToList();

        var alunosComPap = await _alunoPapService.VerificarAlunosPossuemProgramaPapAsync(
            codigosAlunos,
            turma.AnoLetivo,
            cancellationToken);

        var alunosComLinguaPortuguesaSegundaLingua = await _repositorioRespostaAluno
            .VerificarAlunosPossuiLinguaPortuguesaAsync(
                codigosAlunos,
                questaoLinguaPortuguesa,
                cancellationToken);

        var respostasAlunosPorQuestoes = await _repositorioRespostaAluno.ObterRespostasAlunosPorQuestoesAsync(
            codigosAlunos.Select(x => (long)x).ToList(),
            questoesIds.Select(x => (long)x).ToList(),
            sondagemAtiva.Id,
            cancellationToken);

        var respostasAlunosPorQuestoesConvertido =
            respostasAlunosPorQuestoes
                .Where(x => x.Value?.OpcaoRespostaId is not null)
                .GroupBy(x => (
                    CodigoAluno: (int)x.Key.CodigoAluno, x.Key.BimestreId, x.Key.QuestaoId
                ))
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .Select(x => x.Value!)
                        .First()
                );

        var questaoIdPrincipal = questoesAtivas.First(x => x.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua).Id;

        var estudantes = ConstruirEstudantes(
            alunos,
            colunas,
            respostasAlunosPorQuestoesConvertido,
            questaoIdPrincipal,
            alunosComLinguaPortuguesaSegundaLingua,
            alunosComPap)
        .OrderBy(e => e.Nome)
        .ToList();

        var questaoId = questoesAtivas.FirstOrDefault(x => x.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua)?.Id ?? 0;

        var tituloTabelaRespostas = ObterTituloTabelaRespostas(questoesAtivas);

        return new QuestionarioSondagemDto
        {
            QuestaoId = questaoId,
            SondagemId = sondagemAtiva.Id,
            TituloTabelaRespostas = tituloTabelaRespostas,
            Estudantes = estudantes,
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
        var questoesAtivas = await _repositorioQuestao.ObterQuestoesAtivasPorFiltroAsync(
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

    private async Task<IEnumerable<dynamic>> ObterAlunosOuLancarExcecao(int turmaId, CancellationToken cancellationToken)
    {
        var alunos = await _repositorioElasticAluno.ObterAlunosPorIdTurma(turmaId, cancellationToken);

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
                throw new ArgumentException("bimestreId é obrigatório quando existem subperguntas.");

            var colunasPorQuestao = questoesAtivas
                .Where(q => !q.Excluido && q.Tipo != TipoQuestao.QuestaoComSubpergunta)
                .OrderBy(q => q.Ordem)
                .Select(q => new ColunaQuestionarioDto
                {
                    IdCiclo = bimestreId.Value,
                    DescricaoColuna = q.Nome,          
                    PeriodoBimestreAtivo = true,
                    QuestaoSubrespostaId = q.Id,
                    OpcaoResposta = ObterOpcoesRespostasPorQuestao(q.Id, questoesAtivas, true)
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

    private static List<OpcaoRespostaDto> ObterOpcoesRespostasPorQuestao(int questaoId, IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas, bool possuiSubperguntas = false)
    {
        var retorno = questoesAtivas
            .Where(q => q.Id == questaoId)
            .SelectMany(q => q.QuestaoOpcoes)
            .OrderBy(qo => qo.Ordem)
            .Select(qo => new OpcaoRespostaDto
            {
                Id = possuiSubperguntas ? qo.OpcaoRespostaId : qo.Id,
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

    private static List<EstudanteQuestionarioDto> ConstruirEstudantes(
        IEnumerable<dynamic> alunos,
        List<ColunaQuestionarioDto> colunas,
        Dictionary<(int CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> respostasAlunosPorQuestoes,
        long questaoIdPrincipal,
        Dictionary<int, bool> alunosComLinguaPortuguesaSegundaLingua,
        Dictionary<int, bool> alunosComPap)
    {
        var estudantes = new List<EstudanteQuestionarioDto>();

        foreach (var aluno in alunos)
        {
            var codigoAluno = (int)aluno.CodigoAluno;

            var colunasAluno = colunas.Select(c =>
            {
                long questaoIdChave =
                    c.QuestaoSubrespostaId.HasValue
                        ? c.QuestaoSubrespostaId.Value
                        : questaoIdPrincipal;

                int? bimestreIdChave = c.IdCiclo == 0 ? (int?)null : c.IdCiclo;

                var chave = (
                    CodigoAluno: codigoAluno,
                    BimestreId: bimestreIdChave,
                    QuestaoId: questaoIdChave
                );

                var possuiResposta = respostasAlunosPorQuestoes.TryGetValue(chave, out var resposta);

                return new ColunaQuestionarioDto
                {
                    IdCiclo = c.IdCiclo,
                    DescricaoColuna = c.DescricaoColuna,
                    PeriodoBimestreAtivo = c.PeriodoBimestreAtivo,
                    QuestaoSubrespostaId = c.QuestaoSubrespostaId,
                    OpcaoResposta = c.OpcaoResposta,
                    Resposta = possuiResposta && resposta is not null
                        ? new RespostaDto
                        {
                            Id = resposta.Id,
                            OpcaoRespostaId = resposta.OpcaoRespostaId == 0 ? null : resposta.OpcaoRespostaId
                        }
                        : null,
                };
            }).ToList();

            estudantes.Add(new EstudanteQuestionarioDto
            {
                NumeroAlunoChamada = aluno.NumeroAlunoChamada,
                Codigo = codigoAluno,
                Nome = aluno.NomeAluno ?? string.Empty,
                LinguaPortuguesaSegundaLingua = alunosComLinguaPortuguesaSegundaLingua.TryGetValue(codigoAluno, out var lingua) && lingua,
                Pap = alunosComPap.TryGetValue(codigoAluno, out var pap) && pap,
                PossuiDeficiencia = aluno.PossuiDeficiencia == 1,
                Coluna = colunasAluno
            });
        }

        return estudantes;
    }

    private static string ObterTituloTabelaRespostas(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        var primeiraQuestao = questoesAtivas.FirstOrDefault();
        
        return PossuiQuestaoVinculo(questoesAtivas)
            ? questoesAtivas.FirstOrDefault(q => q.QuestaoVinculo?.Tipo == TipoQuestao.QuestaoComSubpergunta)?.QuestaoVinculo?.Nome!
            : primeiraQuestao?.Nome ?? string.Empty;
    }
}
