using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;

public interface IAtualizarQuestaoUseCase
{
    Task<QuestaoDto?> ExecutarAsync(long id, QuestaoDto questaoDto, CancellationToken cancellationToken = default);
}