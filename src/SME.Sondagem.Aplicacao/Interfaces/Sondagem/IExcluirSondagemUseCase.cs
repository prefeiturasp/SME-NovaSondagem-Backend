namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface IExcluirSondagemUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}