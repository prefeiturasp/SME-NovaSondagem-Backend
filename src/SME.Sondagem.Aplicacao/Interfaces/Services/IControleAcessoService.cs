namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IControleAcessoService
    {
        Task<bool> ValidarPermissaoAcessoAsync(string turmaId, CancellationToken cancellationToken = default);
    }
}
