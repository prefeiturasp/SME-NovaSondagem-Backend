using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Dominio.ValueObjects;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class SondagemSalvarRespostasUseCase : ISondagemSalvarRespostasUseCase
{
    private readonly IRepositorioSondagem _repositorioSondagem;
    private readonly IRepositorioRespostaAluno _repositorioSondagemResposta;
    private readonly IRepositorioQuestao _repositorioQuestao;
    private readonly IControleAcessoService _controleAcessoService;
    private readonly IRepositorioElasticTurma _repositorioElasticTurma;
    private readonly IRepositorioElasticAluno _repositorioElasticAluno;
    private readonly IDadosAlunosService _dadosAlunosService;

    public SondagemSalvarRespostasUseCase(IRepositorioSondagem repositorioSondagem,
        IRepositorioRespostaAluno repositorioSondagemResposta,
        IRepositorioQuestao repositorioQuestao,
        IControleAcessoService controleAcessoService,
        IRepositorioElasticTurma repositorioElasticTurma,
        IRepositorioElasticAluno repositorioElasticAluno,
        IDadosAlunosService dadosAlunosService)
    {
        _repositorioSondagem = repositorioSondagem;
        _repositorioSondagemResposta = repositorioSondagemResposta;
        _repositorioQuestao = repositorioQuestao;
        _controleAcessoService = controleAcessoService;
        _repositorioElasticTurma = repositorioElasticTurma;
        _repositorioElasticAluno = repositorioElasticAluno;
        _dadosAlunosService = dadosAlunosService;
    }

    public async Task<bool> SalvarOuAtualizarSondagemAsync(SondagemSalvarDto dto)
    {
        var turma = await ValidarSalvarSondagemEObterTurma(dto);
        dto.AnoTurma ??= int.TryParse(turma.AnoTurma, out var anoTurmaElastic) ? anoTurmaElastic : null;

        var sondagemAtiva = await ObterEValidarSondagemAtiva(dto.SondagemId);
        var alunosParaSalvar = await ObterAlunosAtivosParaSalvar(dto, turma);

        if (alunosParaSalvar.Count == 0)
            return true;

        var dadosRacaGenero = await _dadosAlunosService.ObterDadosRacaGeneroAlunos(Convert.ToInt32(dto.TurmaId));
        var periodosBimestresAtivos = sondagemAtiva.PeriodosBimestre.Where(x => !x.Excluido);

        var alunosIds = alunosParaSalvar.Select(a => a.Codigo);
        var questoesId = alunosParaSalvar.SelectMany(a => a.Respostas.Select(r => r.QuestaoId)) ?? [0];

        var questoes = await _repositorioQuestao.ObterQuestionarioIdPorQuestoesAsync(questoesId) ?? throw new NegocioException(MensagemNegocioComuns.QUESTOES_NAO_PERTENCEM_A_UM_QUESTIONARIO);
        var primeiroQuestionarioId = questoes.FirstOrDefault()!.QuestionarioId;
        var questaoLinguaPortuguesaSegundaLingua = await _repositorioQuestao.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(primeiroQuestionarioId, TipoQuestao.LinguaPortuguesaSegundaLingua);

        var questoesIdsResposta = questoesId.ToList();

        if (questaoLinguaPortuguesaSegundaLingua is not null)
        {
            questoesIdsResposta.Add(questaoLinguaPortuguesaSegundaLingua.Id);
        }

        var repostasAlunos =
            await _repositorioSondagemResposta.ObterRespostasPorSondagemEAlunosAsync(
                dto.SondagemId,
                dto.TurmaId!,
                alunosIds,
                questoesIdsResposta);

        var respostas = ProcessarRespostasAlunos(dto, alunosParaSalvar, periodosBimestresAtivos, repostasAlunos, questaoLinguaPortuguesaSegundaLingua, dadosRacaGenero);

        if (respostas.Count == 0)
            return true;

        return await _repositorioSondagemResposta.SalvarAsync(respostas);
    }

    private async Task<List<AlunoSondagemDto>> ObterAlunosAtivosParaSalvar(SondagemSalvarDto dto, TurmaElasticDto turma)
    {
        var turmaId = Convert.ToInt32(dto.TurmaId);
        var anoLetivo = dto.AnoLetivo != 0 ? dto.AnoLetivo : turma.AnoLetivo;

        var alunosTurma = await _repositorioElasticAluno.ObterAlunosPorIdTurma(turmaId, anoLetivo, CancellationToken.None);
        var codigosAlunosAtivos = alunosTurma
            .Where(a => a.CodigoSituacaoMatricula == (int)SituacaoMatriculaAluno.Ativo)
            .Select(a => a.CodigoAluno)
            .ToHashSet();

        return dto.Alunos
            .Where(a => codigosAlunosAtivos.Contains(a.Codigo))
            .ToList();
    }

    private async Task<TurmaElasticDto> ValidarSalvarSondagemEObterTurma(SondagemSalvarDto dto)
    {
        if (dto == null)
            throw new RegraNegocioException(MensagemNegocioComuns.INFORMAR_DADOS_SALVAR_SONDAGEM);

        ValidarCamposObrigatorios(dto);
        var filtro = new FiltroQuestionario
        {
            TurmaId = int.TryParse(dto.TurmaId, out var turmaId) ? turmaId : 0
        };

        var turma = await _repositorioElasticTurma.ObterTurmaPorId(filtro, default)
            ?? throw new RegraNegocioException("Turma não localizada", 400);

        var codigoEscola = turma.CodigoEscola;
        var anoTurma = turma.AnoTurma;

        if (string.IsNullOrEmpty(dto.TurmaId))
            throw new RegraNegocioException(MensagemNegocioComuns.INFORMAR_TURMA_SALVAR_SONDAGEM);

        bool temPermissao = await _controleAcessoService.ValidarPermissaoAcessoAsync(dto.TurmaId, codigoEscola, anoTurma);

        if (!temPermissao)
            throw new RegraNegocioException(MensagemNegocioComuns.SEM_PERMISSAO_SALVAR_SONDAGEM);

        return turma;
    }

    private static void ValidarCamposObrigatorios(SondagemSalvarDto dto)
    {
        var modalidadeId = dto.ModalidadeId ?? 0;
        var modalidadeValida = Enum.IsDefined(typeof(Modalidade), modalidadeId);

        var regras = new (Func<SondagemSalvarDto, bool> Invalido, string Mensagem)[]
        {
        (d => string.IsNullOrWhiteSpace(d.TurmaId) || d.TurmaId == "0",
            MensagemNegocioComuns.INFORMAR_TURMA_SALVAR_SONDAGEM),

        (d => string.IsNullOrWhiteSpace(d.UeId) || d.UeId == "0",
            MensagemNegocioComuns.INFORMAR_UE_SALVAR_SONDAGEM),

        (d => string.IsNullOrWhiteSpace(d.DreId) || d.DreId == "0",
            MensagemNegocioComuns.INFORMAR_DRE_SALVAR_SONDAGEM),

        (d => d.AnoLetivo == 0,
            MensagemNegocioComuns.INFORMAR_ANO_LETIVO_SALVAR_SONDAGEM),


        (d => !modalidadeValida,
            MensagemNegocioComuns.INFORMAR_MODALIDADE_SALVAR_SONDAGEM),
        };

        foreach (var (invalido, mensagem) in regras)
        {
            if (invalido(dto))
                throw new NegocioException(mensagem);
        }
    }

    private async Task<Dominio.Entidades.Sondagem.Sondagem> ObterEValidarSondagemAtiva(int sondagemId)
    {
        var sondagemAtiva = await _repositorioSondagem.ObterSondagemAtiva();

        if (sondagemAtiva == null)
            throw new NegocioException(MensagemNegocioComuns.NENHUM_SONDAGEM_ATIVA_ENCONRADA);

        return sondagemAtiva.Id != sondagemId
            ? throw new NegocioException(MensagemNegocioComuns.SALVAR_SOMENTE_PARA_SONDAGEM_ATIVA)
            : sondagemAtiva;
    }

    private static List<RespostaAluno> ProcessarRespostasAlunos(
        SondagemSalvarDto dto,
        IEnumerable<AlunoSondagemDto> alunos,
        IEnumerable<SondagemPeriodoBimestre> periodosBimestresAtivos,
        IEnumerable<RespostaAluno> respostasExistentes,
        Dominio.Entidades.Questionario.Questao? questaoLinguaPortuguesa,
        IEnumerable<Infrastructure.Dtos.AlunoRacaGeneroDto> dadosRacaGenero)
    {
        var respostas = new List<RespostaAluno>();
        var racaGeneroPorAluno = dadosRacaGenero
                        .ToDictionary(d => d.CodigoAluno);

        foreach (var aluno in alunos)
        {
            racaGeneroPorAluno.TryGetValue(aluno.Codigo, out var racaGenero);
            var contexto = CriarContexto(dto, racaGenero, aluno);
            if (questaoLinguaPortuguesa is not null)
            {
                var respostaLinguaPortuguesa = ProcessarRespostaLinguaPortuguesa(
                    dto.SondagemId,
                    aluno,
                    questaoLinguaPortuguesa,
                    respostasExistentes,
                    contexto);

                if (respostaLinguaPortuguesa is not null)
                    respostas.Add(respostaLinguaPortuguesa);
            }

            foreach (var respostaDto in aluno.Respostas)
            {
                var resposta = ProcessarRespostaIndividual(
                    dto.SondagemId,
                    aluno.Codigo,
                    respostaDto,
                    periodosBimestresAtivos,
                    respostasExistentes, contexto);

                if (resposta != null)
                    respostas.Add(resposta);
            }
        }

        return respostas;
    }

    private static RespostaAluno? ProcessarRespostaLinguaPortuguesa(
    int sondagemId,
    AlunoSondagemDto aluno,
    Dominio.Entidades.Questionario.Questao questao,
    IEnumerable<RespostaAluno> respostasExistentes, ContextoEducacional contexto)
    {
        var respostaExistente = respostasExistentes.FirstOrDefault(r =>
            r.AlunoId == aluno.Codigo &&
            r.QuestaoId == questao.Id &&
            r.TurmaId == contexto.TurmaId);

        var descricao = aluno.LinguaPortuguesaSegundaLingua ? "sim" : "nao";

        var opcaoRespostaId = questao.QuestaoOpcoes
            .FirstOrDefault(o => o.OpcaoResposta.DescricaoOpcaoResposta
                .Equals(descricao, StringComparison.OrdinalIgnoreCase))
            ?.OpcaoRespostaId;

        if (respostaExistente != null)
        {
            return respostaExistente.AtualizarResposta(
                opcaoRespostaId,
                DateTimeExtension.HorarioBrasilia(),
                contexto)
                ? respostaExistente
                : null;
        }

        return new RespostaAluno(
            sondagemId,
            aluno.Codigo,
            questao.Id,
            opcaoRespostaId,
            DateTimeExtension.HorarioBrasilia(), contexto);
    }

    private static RespostaAluno? ProcessarRespostaIndividual(
        int sondagemId,
        int alunoId,
        RespostaSondagemDto respostaDto,
        IEnumerable<SondagemPeriodoBimestre> periodosBimestresAtivos,
        IEnumerable<RespostaAluno> repostasAlunos, ContextoEducacional contexto
        )
    {
        var periodoBimestreAtivo = periodosBimestresAtivos
            .FirstOrDefault(pb => pb.BimestreId == respostaDto.BimestreId);

        if (!ValidarPeriodoBimestre(periodoBimestreAtivo))
            return null;

        var respostaExistente = repostasAlunos?.FirstOrDefault(r =>
            r.AlunoId == alunoId &&
            r.QuestaoId == respostaDto.QuestaoId &&
            r.BimestreId == respostaDto.BimestreId &&
            r.TurmaId == contexto.TurmaId);

        return CriarOuAtualizarResposta(
            sondagemId,
            alunoId,
            respostaDto,
            respostaExistente, contexto);
    }

    private static bool ValidarPeriodoBimestre(SondagemPeriodoBimestre? periodoBimestreAtivo)
    {
        if (periodoBimestreAtivo == null)
            return false;

        return periodoBimestreAtivo.DataInicio <= DateTime.Now &&
               periodoBimestreAtivo.DataFim >= DateTime.Now;
    }

    private static RespostaAluno? CriarOuAtualizarResposta(
        int sondagemId,
        int alunoId,
        RespostaSondagemDto respostaDto,
        RespostaAluno? respostaExistente, ContextoEducacional contexto
        )
    {
        if (respostaExistente == null && !respostaDto.OpcaoRespostaId.HasValue)
            return null;

        if (respostaExistente == null)
            return new RespostaAluno(
                sondagemId,
                alunoId,
                respostaDto.QuestaoId,
                respostaDto.OpcaoRespostaId,
                DateTime.UtcNow,
                contexto with { BimestreId = respostaDto.BimestreId });

        return respostaExistente.AtualizarResposta(
            respostaDto.OpcaoRespostaId,
            DateTime.UtcNow,
            contexto)
            ? respostaExistente
            : null;
    }

    private static ContextoEducacional CriarContexto(SondagemSalvarDto dto, Infrastructure.Dtos.AlunoRacaGeneroDto? racaGenero, AlunoSondagemDto aluno) =>
    new()
    {
        TurmaId = dto.TurmaId,
        UeId = dto.UeId,
        DreId = dto.DreId,
        AnoLetivo = dto.AnoLetivo,
        ModalidadeId = dto.ModalidadeId,
        RacaCorId = racaGenero?.RacaId,
        GeneroSexoId = racaGenero?.SexoId,
        Pap = aluno.Pap,
        Deficiente = aluno.Deficiente,
        Aee = aluno.Aee,
        AnoTurma = dto.AnoTurma,
        SemestreId = CalcularSemestreId(dto.ModalidadeId)
    };

    private static int? CalcularSemestreId(int? modalidadeId)
    {
        if (modalidadeId != (int)Modalidade.EJA)
            return null;

        return DateTimeExtension.HorarioBrasilia().Semestre();
    }

}
