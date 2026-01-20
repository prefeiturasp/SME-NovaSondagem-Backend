namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IExcluirQuestionarioUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}