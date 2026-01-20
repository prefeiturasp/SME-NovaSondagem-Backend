using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;

public interface ICriarQuestaoOpcaoRespostaUseCase
{
    Task<long> ExecutarAsync(QuestaoOpcaoRespostaDto questaoOpcaoRespostaDto, CancellationToken cancellationToken = default);
}