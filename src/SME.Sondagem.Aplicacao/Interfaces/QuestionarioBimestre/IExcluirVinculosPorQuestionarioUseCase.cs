namespace SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;

public interface IExcluirVinculosPorQuestionarioUseCase
{
    Task<bool> ExecutarAsync(int questionarioId, CancellationToken cancellationToken = default);
}