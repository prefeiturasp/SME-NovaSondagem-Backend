namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IControleAcessoService
    {
        Task<bool> ValidarPermissaoAcessoAsync(string turmaId, string codigoEscola = "", CancellationToken cancellationToken = default);
    }
}
