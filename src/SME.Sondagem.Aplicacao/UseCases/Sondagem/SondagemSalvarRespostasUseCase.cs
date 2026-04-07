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
    private readonly IDadosAlunosService _dadosAlunosService;

    public SondagemSalvarRespostasUseCase(IRepositorioSondagem repositorioSondagem,
        IRepositorioRespostaAluno repositorioSondagemResposta,
        IRepositorioQuestao repositorioQuestao,
        IControleAcessoService controleAcessoService,
        IRepositorioElasticTurma repositorioElasticTurma,
        IDadosAlunosService dadosAlunosService)
    {
        _repositorioSondagem = repositorioSondagem;
        _repositorioSondagemResposta = repositorioSondagemResposta;
        _repositorioQuestao = repositorioQuestao;
        _controleAcessoService = controleAcessoService;
        _repositorioElasticTurma = repositorioElasticTurma;
        _dadosAlunosService = dadosAlunosService;
    }

    public async Task<bool> SalvarOuAtualizarSondagemAsync(SondagemSalvarDto dto)
    {
        await ValidarSalvarSondagem(dto);
        var dadosRacaGenero = await _dadosAlunosService.ObterDadosRacaGeneroAlunos(Convert.ToInt32(dto.TurmaId));

        var sondagemAtiva = await ObterEValidarSondagemAtiva(dto.SondagemId);
        var periodosBimestresAtivos = sondagemAtiva.PeriodosBimestre.Where(x => !x.Excluido);

        var alunosIds = dto.Alunos.Select(a => a.Codigo);
        var questoesId = dto.Alunos.SelectMany(a => a.Respostas.Select(r => r.QuestaoId)) ?? [0];

        var questoes = await _repositorioQuestao.ObterQuestionarioIdPorQuestoesAsync(questoesId) ?? throw new NegocioException(MensagemNegocioComuns.QUESTOES_NAO_PERTENCEM_A_UM_QUESTIONARIO);
        var primeiroQuestionarioId = questoes.FirstOrDefault()!.QuestionarioId;
        var questaoLinguaPortuguesaSegundaLingua = await _repositorioQuestao.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(primeiroQuestionarioId, TipoQuestao.LinguaPortuguesaSegundaLingua);

        var questoesIdsResposta = questoesId.ToList();

        if (questaoLinguaPortuguesaSegundaLingua is not null)
        {
            questoesIdsResposta.Add(questaoLinguaPortuguesaSegundaLingua.Id);
        }

        var repostasAlunos =
            await _repositorioSondagemResposta.ObterRespostasPorSondagemEAlunosAsync(dto.SondagemId, alunosIds, questoesIdsResposta);

        var respostas = ProcessarRespostasAlunos(dto, periodosBimestresAtivos, repostasAlunos, questaoLinguaPortuguesaSegundaLingua, dadosRacaGenero);

        return await _repositorioSondagemResposta.SalvarAsync(respostas);
    }

    private async Task ValidarSalvarSondagem(SondagemSalvarDto dto)
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
    }

    private static void ValidarCamposObrigatorios(SondagemSalvarDto dto)
    {

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

        (d => string.IsNullOrWhiteSpace(d.ModalidadeId) || !Enum.IsDefined(typeof(Modalidade),Convert.ToInt32(dto.ModalidadeId)),
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
        IEnumerable<SondagemPeriodoBimestre> periodosBimestresAtivos,
        IEnumerable<RespostaAluno> respostasExistentes,
        Dominio.Entidades.Questionario.Questao? questaoLinguaPortuguesa,
        IEnumerable<Infrastructure.Dtos.AlunoRacaGeneroDto> dadosRacaGenero)
    {
        var respostas = new List<RespostaAluno>();
        var racaGeneroPorAluno = dadosRacaGenero
                        .ToDictionary(d => d.CodigoAluno);


        foreach (var aluno in dto.Alunos)
        {
            racaGeneroPorAluno.TryGetValue(aluno.Codigo, out var racaGenero);
            var contexto = CriarContexto(dto, aluno.ProgramaAtendimentoId,racaGenero);
            if (questaoLinguaPortuguesa is not null)
            {
                respostas.Add(ProcessarRespostaLinguaPortuguesa(
                    dto.SondagemId,
                    aluno,
                    questaoLinguaPortuguesa,
                    respostasExistentes, contexto));
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

    private static RespostaAluno ProcessarRespostaLinguaPortuguesa(
    int sondagemId,
    AlunoSondagemDto aluno,
    Dominio.Entidades.Questionario.Questao questao,
    IEnumerable<RespostaAluno> respostasExistentes, ContextoEducacional contexto)
    {
        var respostaExistente = respostasExistentes.FirstOrDefault(r =>
            r.AlunoId == aluno.Codigo &&
            r.QuestaoId == questao.Id);

        var descricao = aluno.LinguaPortuguesaSegundaLingua ? "sim" : "nao";

        var opcaoRespostaId = questao.QuestaoOpcoes
            .FirstOrDefault(o => o.OpcaoResposta.DescricaoOpcaoResposta
                .Equals(descricao, StringComparison.OrdinalIgnoreCase))
            ?.OpcaoRespostaId;

        if (respostaExistente != null)
        {
            respostaExistente.AtualizarResposta(opcaoRespostaId, DateTimeExtension.HorarioBrasilia(), contexto);
            return respostaExistente;
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
            r.AlunoId == alunoId && r.QuestaoId == respostaDto.QuestaoId && r.BimestreId == respostaDto.BimestreId);

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

    private static RespostaAluno CriarOuAtualizarResposta(
        int sondagemId,
        int alunoId,
        RespostaSondagemDto respostaDto,
        RespostaAluno? respostaExistente, ContextoEducacional contexto
        )
    {
        if (respostaExistente == null)
            return new RespostaAluno(
                sondagemId,
                alunoId,
                respostaDto.QuestaoId,
                respostaDto.OpcaoRespostaId,
                DateTime.UtcNow,
                contexto with { BimestreId = respostaDto.BimestreId });

        respostaExistente.AtualizarResposta(respostaDto.OpcaoRespostaId, DateTime.UtcNow, contexto);
        return respostaExistente;
    }

    private static ContextoEducacional CriarContexto(SondagemSalvarDto dto, int? programaAtendimentoId, Infrastructure.Dtos.AlunoRacaGeneroDto? racaGenero) =>
    new()
    {
        TurmaId = dto.TurmaId,
        UeId = dto.UeId,
        DreId = dto.DreId,
        AnoLetivo = dto.AnoLetivo,
        ModalidadeId = dto.ModalidadeId,
        RacaCorId = racaGenero?.RacaId,
        GeneroSexoId = racaGenero?.SexoId,
        ProgramaAtendimentoId = programaAtendimentoId
    };

}