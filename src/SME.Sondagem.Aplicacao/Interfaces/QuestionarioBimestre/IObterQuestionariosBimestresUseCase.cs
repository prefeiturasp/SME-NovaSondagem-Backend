using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

namespace SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;

public interface IObterQuestionariosBimestresUseCase
{
    Task<IEnumerable<QuestionarioBimestreDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}