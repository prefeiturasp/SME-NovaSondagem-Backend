using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Questionario;
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

        //obter questionarioId do com base questoresIds
        var questionarioId = await _repositorioQuestao.ObterQuestionarioIdPorQuestoesAsync(questoesId);
        if (questionarioId == null)
            throw new NegocioException(MensagemNegocioComuns.QUESTOES_NAO_PERTENCEM_A_UM_QUESTIONARIO);

        var questaoLinguaPortuguesaSegundaLingua = await _repositorioQuestao.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(questionarioId.Value,TipoQuestao.LinguaPortuguesaSegundaLingua);
        //obter questao lingua portuguesa não excluida a partir do questionarioId e do tipo TipoQuestao.LinguaPortuguesaSegundaLingua
        //adicionar o id dessa questao na lista de questoesIds para poder buscar as respostas dos alunos para essa questao
        //criar variavel questaoLinguaPortuguesaSegundaLinguaId e passar para o ProcessarRespostasAlunos

        if (questaoLinguaPortuguesaSegundaLingua == null)
            throw new NegocioException(MensagemNegocioComuns.QUESTAO_NAO_ENCONTRADA);

        var questoesIdsResposta = questoesId.ToList();
        questoesIdsResposta.Add(questaoLinguaPortuguesaSegundaLingua.Id);

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
        IEnumerable<RespostaAluno> repostasAlunos,
        Dominio.Entidades.Questionario.Questao questaoLinguaPortugues)
    {
        var respostas = new List<RespostaAluno>();

        foreach (var aluno in dto.Alunos)
        {
            var repostaLinguaPortuguesaSegundaLinguaDto = repostasAlunos
                .FirstOrDefault(r => r.AlunoId == aluno.Codigo && r.QuestaoId == questaoLinguaPortugues.Id);

            int? respostaLinguaPortuguesaSegundaLinguaExistente = null;
            if (aluno.LinguaPortuguesaSegundaLingua)
            {
                respostaLinguaPortuguesaSegundaLinguaExistente = questaoLinguaPortugues.QuestaoOpcoes.FirstOrDefault(o => o.OpcaoResposta.DescricaoOpcaoResposta.ToLower() == "sim")?.Id;
            }
            else
            {
                respostaLinguaPortuguesaSegundaLinguaExistente = questaoLinguaPortugues.QuestaoOpcoes.FirstOrDefault(o => o.OpcaoResposta.DescricaoOpcaoResposta.ToLower() == "nao")?.Id;
            }

            if(repostaLinguaPortuguesaSegundaLinguaDto is not null)
            {
                repostaLinguaPortuguesaSegundaLinguaDto.AtualizarResposta(
                    respostaLinguaPortuguesaSegundaLinguaExistente.Value,
                    DateTime.UtcNow);

                respostas.Add(repostaLinguaPortuguesaSegundaLinguaDto);
            }
            else
            {
                var novaRespostaLinguaPortuguesaSegundaLingua = new RespostaAluno(
                    dto.SondagemId,
                    aluno.Codigo,
                    questaoLinguaPortugues.Id,
                    respostaLinguaPortuguesaSegundaLinguaExistente.Value,
                    DateTime.UtcNow,
                    null);

                respostas.Add(novaRespostaLinguaPortuguesaSegundaLingua);
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
                        repostasAlunos);

                    if (resposta != null)
                        respostas.Add(resposta);
                }
        }

        return respostas;
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
            r.AlunoId == alunoId && r.QuestaoId == respostaDto.QuestaoId);

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
                respostaDto.OpcaoRepostaId,
                DateTime.UtcNow,
                respostaDto.BimestreId);
        respostaExistente.AtualizarResposta(respostaDto.OpcaoRepostaId, DateTime.UtcNow);
        return respostaExistente;
    }
}