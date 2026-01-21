using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;

public interface IObterQuestaoOpcaoRespostaPorIdUseCase
{
    Task<QuestaoOpcaoRespostaDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}