using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

namespace SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;

public interface IObterBimestresPorQuestionarioUseCase
{
    Task<IEnumerable<QuestionarioBimestreDto>> ExecutarAsync(int questionarioId, CancellationToken cancellationToken = default);
}