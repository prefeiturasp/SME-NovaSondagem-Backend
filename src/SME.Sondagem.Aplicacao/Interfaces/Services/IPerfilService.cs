using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IPerfilService
    {
        Task<PerfilInfo?> ObterPerfilPorIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
