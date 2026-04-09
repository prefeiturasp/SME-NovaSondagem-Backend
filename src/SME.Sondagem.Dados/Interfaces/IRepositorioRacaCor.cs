using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Interfaces
{
    public interface IRepositorioRacaCor : IRepositorioBase<RacaCor>
    {
        Task<RacaCor?> ObterPorCodigoEolAsync(int codigoEol, CancellationToken cancellationToken = default);
    }
}
