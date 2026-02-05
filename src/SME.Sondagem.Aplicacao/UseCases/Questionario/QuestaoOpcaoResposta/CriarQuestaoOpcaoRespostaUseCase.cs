using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;

public class CriarQuestaoOpcaoRespostaUseCase : ICriarQuestaoOpcaoRespostaUseCase
{
    private readonly IRepositorioQuestaoOpcaoResposta questaoOpcaoRespostaRepositorio;

    public CriarQuestaoOpcaoRespostaUseCase(IRepositorioQuestaoOpcaoResposta questaoOpcaoRespostaRepositorio)
    {
        this.questaoOpcaoRespostaRepositorio = questaoOpcaoRespostaRepositorio;
    }

    public async Task<long> ExecutarAsync(QuestaoOpcaoRespostaDto questaoOpcaoRespostaDto, CancellationToken cancellationToken = default)
    {
        var questaoOpcaoResposta = new SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta(
            questaoOpcaoRespostaDto.QuestaoId,
            questaoOpcaoRespostaDto.OpcaoRespostaId,
            questaoOpcaoRespostaDto.Ordem);

        return await questaoOpcaoRespostaRepositorio.SalvarAsync(questaoOpcaoResposta, cancellationToken: cancellationToken);
    }
}