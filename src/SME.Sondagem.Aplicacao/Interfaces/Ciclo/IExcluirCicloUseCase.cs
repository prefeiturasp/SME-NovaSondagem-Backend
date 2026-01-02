namespace SME.Sondagem.Aplicacao.Interfaces.Ciclo;

public interface IExcluirCicloUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}