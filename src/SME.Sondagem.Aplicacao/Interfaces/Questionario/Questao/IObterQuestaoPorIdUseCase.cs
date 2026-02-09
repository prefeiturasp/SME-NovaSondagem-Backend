using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;

public interface IObterQuestaoPorIdUseCase
{
    Task<QuestaoDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}