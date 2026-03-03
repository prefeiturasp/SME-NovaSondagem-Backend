namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;

public interface IExcluirParametroSondagemUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}