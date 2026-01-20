using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class SondagemSalvarRespostasUseCase : ISondagemSalvarRespostasUseCase
{
    private readonly IRepositorioSondagem _repositorioSondagem;
    private readonly IRepositorioRespostaAluno _repositorioSondagemResposta;
    private readonly IRepositorioQuestao _repositorioQuestao;

    public SondagemSalvarRespostasUseCase(IRepositorioSondagem repositorioSondagem,
        IRepositorioRespostaAluno repositorioSondagemResposta,
        IRepositorioQuestao repositorioQuestao)
    {
        _repositorioSondagem = repositorioSondagem;
        _repositorioSondagemResposta = repositorioSondagemResposta;
        _repositorioQuestao = repositorioQuestao;
    }

    public async Task<bool> SalvarOuAtualizarSondagemAsync(SondagemSalvarDto dto)
    {
        var sondagemAtiva = await ObterEValidarSondagemAtiva(dto.SondagemId);
        var periodosBimestresAtivos = sondagemAtiva.PeriodosBimestre.Where(x => !x.Excluido);

        var alunosIds = dto.Alunos.Select(a => a.Codigo);
        var questoesId = dto.Alunos.SelectMany(a => a.Respostas.Select(r => r.QuestaoId)) ?? [0];

        var questoes = await _repositorioQuestao.ObterQuestionarioIdPorQuestoesAsync(questoesId);
        if (questoes == null)
            throw new NegocioException(MensagemNegocioComuns.QUESTOES_NAO_PERTENCEM_A_UM_QUESTIONARIO);

        var primeiroQuestionarioId = questoes.FirstOrDefault()!.QuestionarioId;
        var questaoLinguaPortuguesaSegundaLingua = await _repositorioQuestao.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(primeiroQuestionarioId, TipoQuestao.LinguaPortuguesaSegundaLingua);

        if (questaoLinguaPortuguesaSegundaLingua == null && questoes.Any(q => q.Tipo != TipoQuestao.QuestaoComSubpergunta))
            throw new NegocioException(MensagemNegocioComuns.QUESTAO_NAO_ENCONTRADA);

        var questoesIdsResposta = questoesId.ToList();

        if (questaoLinguaPortuguesaSegundaLingua is not null)
        {
            questoesIdsResposta.Add(questaoLinguaPortuguesaSegundaLingua.Id);
        }

        var repostasAlunos =
            await _repositorioSondagemResposta.ObterRespostasPorSondagemEAlunosAsync(dto.SondagemId, alunosIds, questoesIdsResposta);

        var respostas = ProcessarRespostasAlunos(dto, periodosBimestresAtivos, repostasAlunos, questaoLinguaPortuguesaSegundaLingua);

        return await _repositorioSondagemResposta.SalvarAsync(respostas);
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
        Dominio.Entidades.Questionario.Questao? questaoLinguaPortuguesa)
    {
        var respostas = new List<RespostaAluno>();

        foreach (var aluno in dto.Alunos)
        {
            if (questaoLinguaPortuguesa is not null)
            {
                respostas.Add(ProcessarRespostaLinguaPortuguesa(
                    dto.SondagemId,
                    aluno,
                    questaoLinguaPortuguesa,
                    respostasExistentes));
            }

            // verificar se o aluno possui resposta para a questao questaoLinguaPortuguesaSegundaLinguaId
            // caso possuo vamos apenas editar a resposta, senao vamos criar uma nova resposta para essa questaoLinguaPortuguesaSegundaLinguaId
            // caso o aluno.LinguaPortuguesaSegundaLingua for true a opcaoRepostaId será a que a descricao for "Sim", senao será a que a descricao for "Nao"
            // adicionar na lista de respostas

            foreach (var respostaDto in aluno.Respostas)
            {
                var resposta = ProcessarRespostaIndividual(
                    dto.SondagemId,
                    aluno.Codigo,
                    respostaDto,
                    periodosBimestresAtivos,
                    respostasExistentes);

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
    IEnumerable<RespostaAluno> respostasExistentes)
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
            respostaExistente.AtualizarResposta(opcaoRespostaId, DateTimeExtension.HorarioBrasilia());
            return respostaExistente;
        }

        return new RespostaAluno(
            sondagemId,
            aluno.Codigo,
            questao.Id,
            opcaoRespostaId,
            DateTimeExtension.HorarioBrasilia(),
            null);
    }

    private static RespostaAluno? ProcessarRespostaIndividual(
        int sondagemId,
        int alunoId,
        RespostaSondagemDto respostaDto,
        IEnumerable<SondagemPeriodoBimestre> periodosBimestresAtivos,
        IEnumerable<RespostaAluno> repostasAlunos)
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
            respostaExistente);
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
        RespostaAluno? respostaExistente)
    {
        if (respostaExistente == null)
            return new RespostaAluno(
                sondagemId,
                alunoId,
                respostaDto.QuestaoId,
                respostaDto.OpcaoRespostaId,
                DateTime.UtcNow,
                respostaDto.BimestreId);
        respostaExistente.AtualizarResposta(respostaDto.OpcaoRespostaId, DateTime.UtcNow);
        return respostaExistente;
    }
}