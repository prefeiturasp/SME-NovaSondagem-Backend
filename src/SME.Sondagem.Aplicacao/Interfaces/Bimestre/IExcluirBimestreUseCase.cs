namespace SME.Sondagem.Aplicacao.Interfaces.Bimestre;

public interface IExcluirBimestreUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}