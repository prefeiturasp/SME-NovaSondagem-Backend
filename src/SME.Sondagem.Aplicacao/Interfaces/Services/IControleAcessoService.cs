namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IControleAcessoService
    {
        Task<bool> ValidarPermissaoAcessoAsync(string turmaId, string codigoEscola = "", string anoTurma = "", CancellationToken cancellationToken = default);
    }
}
