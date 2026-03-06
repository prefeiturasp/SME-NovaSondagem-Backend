using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Base;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ObterQuestionarioSondagemUseCase : QuestionarioSondagemUseCaseBase, IObterQuestionarioSondagemUseCase
{
    public ObterQuestionarioSondagemUseCase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        IAlunoPapService alunoPapService,
        IControleAcessoService controleAcessoService,
        IServicoUsuario servicoUsuario)
        : base(repositoriosElastic, repositoriosSondagem, alunoPapService, controleAcessoService, servicoUsuario)
    {
    }

    public async Task<QuestionarioSondagemDto> ObterQuestionarioSondagem([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        var turma = await ValidarFiltroEModalidade(filtro, cancellationToken);

        var sondagemAtiva = await ObterSondagemAtivaOuLancarExcecao(cancellationToken);

        return await ProcessarQuestionario(filtro, turma, sondagemAtiva, cancellationToken);
    }

    private async Task<Dominio.Entidades.Sondagem.Sondagem> ObterSondagemAtivaOuLancarExcecao(CancellationToken cancellationToken)
    {
        return await _repositoriosSondagem.RepositorioSondagem.ObterSondagemAtiva(cancellationToken)
            ?? throw new ErroInternoException("Não há sondagem ativa cadastrada no sistema");
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
            throw new ErroInternoException("Ano da turma inválido");

        ValidarModalidadeEAno(modalidade, ano);

        var questoesAtivas = await ObterQuestoesAtivasOuLancarExcecao(modalidade, ano, anoLetivo, filtro.ProficienciaId, cancellationToken);

        var questaoLinguaPortuguesa = questoesAtivas.FirstOrDefault(x => x.Tipo == TipoQuestao.LinguaPortuguesaSegundaLingua);

        var questoesIds = ObterQuestoesIdsPorTipo(questoesAtivas);

        var alunos = await ObterAlunosOuLancarExcecao(filtro.TurmaId, anoLetivo, cancellationToken);

        var bimestresForaDoPadrao = await _repositoriosSondagem.RepositorioBimestre
            .ObterBimestresPorQuestionarioIdAsync(ObterIdQuestionario(questoesAtivas), cancellationToken);

        var colunas = await ObterColunasOuLancarExcecao(bimestresForaDoPadrao != null && bimestresForaDoPadrao.Count > 0 ? bimestresForaDoPadrao : sondagemAtiva.PeriodosBimestre, questoesAtivas, filtro.BimestreId);

        var codigosAlunos = alunos.Select(a => (int)a.CodigoAluno).ToList();

        var alunosComPap = await _alunoPapService.VerificarAlunosPossuemProgramaPapAsync(
            codigosAlunos,
            anoLetivo,
            cancellationToken);

        var alunosComLinguaPortuguesaSegundaLingua = await _repositoriosSondagem.RepositorioRespostaAluno
            .VerificarAlunosPossuiLinguaPortuguesaAsync(
                codigosAlunos,
                questaoLinguaPortuguesa,
                cancellationToken);

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
            QuestionarioId = ObterIdQuestionario(questoesAtivas),
            QuestaoId = questaoId,
            SondagemId = sondagemAtiva.Id,
            TituloTabelaRespostas = tituloTabelaRespostas,
            PodeSalvar = await _controleAcessoService.ValidarPermissaoAcessoAsync(turma.CodigoTurma.ToString(), cancellationToken),
            Estudantes = estudantes,
            InseridoPor = nomeInseridoPor,
            AlteradoPor = nomeAlteradoPor
        };
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

            var colunasAluno = colunas
                .Select(c => ConstruirColunaAluno(c, codigoAluno, questaoIdPrincipal, respostasAlunosPorQuestoes))
                .ToList();

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

    private static ColunaQuestionarioDto ConstruirColunaAluno(
        ColunaQuestionarioDto colunaBase,
        int codigoAluno,
        long questaoIdPrincipal,
        Dictionary<(int CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> respostasAlunosPorQuestoes)
    {
        long questaoIdChave = colunaBase.QuestaoSubrespostaId.HasValue
            ? colunaBase.QuestaoSubrespostaId.Value
            : questaoIdPrincipal;

        int? bimestreIdChave = colunaBase.IdCiclo == 0 ? (int?)null : colunaBase.IdCiclo;

        var chave = (
            CodigoAluno: codigoAluno,
            BimestreId: bimestreIdChave,
            QuestaoId: questaoIdChave
        );

        var possuiResposta = respostasAlunosPorQuestoes.TryGetValue(chave, out var resposta);

        return new ColunaQuestionarioDto
        {
            IdCiclo = colunaBase.IdCiclo,
            DescricaoColuna = colunaBase.DescricaoColuna,
            PeriodoBimestreAtivo = colunaBase.PeriodoBimestreAtivo,
            QuestaoSubrespostaId = colunaBase.QuestaoSubrespostaId,
            OpcaoResposta = colunaBase.OpcaoResposta,
            Resposta = ConstruirResposta(possuiResposta, resposta)
        };
    }

    protected override async Task<DadosAlunosDto> ObterDadosAlunos(
        int turmaId,
        int anoLetivo,
        ContextoProcessamentoDto contexto,
        CancellationToken cancellationToken)
    {
        var alunosComPap = await _alunoPapService.VerificarAlunosPossuemProgramaPapAsync(
            contexto.CodigosAlunos,
            anoLetivo,
            cancellationToken);

        var alunosComLinguaPortuguesaSegundaLingua = await _repositoriosSondagem.RepositorioRespostaAluno
            .VerificarAlunosPossuiLinguaPortuguesaAsync(
                contexto.CodigosAlunos,
                contexto.QuestaoLinguaPortuguesa,
                cancellationToken);

        return new DadosAlunosDto
        {
            AlunosComPap = alunosComPap,
            AlunosComLinguaPortuguesaSegundaLingua = alunosComLinguaPortuguesaSegundaLingua,
            DadosRacaGenero = null
        };
     }
 }