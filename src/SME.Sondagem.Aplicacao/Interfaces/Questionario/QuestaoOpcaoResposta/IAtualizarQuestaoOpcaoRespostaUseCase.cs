using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;

public interface IAtualizarQuestaoOpcaoRespostaUseCase
{
    Task<QuestaoOpcaoRespostaDto?> ExecutarAsync(long id, QuestaoOpcaoRespostaDto questaoOpcaoRespostaDto, CancellationToken cancellationToken = default);
}