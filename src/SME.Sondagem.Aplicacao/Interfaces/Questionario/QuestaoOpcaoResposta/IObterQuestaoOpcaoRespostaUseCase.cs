using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;

public interface IObterQuestaoOpcaoRespostaUseCase
{
    Task<IEnumerable<QuestaoOpcaoRespostaDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}