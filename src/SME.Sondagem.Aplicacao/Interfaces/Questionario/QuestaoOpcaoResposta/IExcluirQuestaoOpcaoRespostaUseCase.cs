namespace SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;

public interface IExcluirQuestaoOpcaoRespostaUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}