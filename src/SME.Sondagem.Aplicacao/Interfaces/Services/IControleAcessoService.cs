namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IControleAcessoService
    {
        Task<bool> ValidarPermissaoAcessoAsync(CancellationToken cancellationToken = default);
    }
}
