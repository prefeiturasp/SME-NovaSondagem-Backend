using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;

public interface IRepositorioComponenteCurricular : IRepositorioBase<ComponenteCurricular>
{
    Task<ComponenteCurricular?> ObterPorCodigoEolAsync(int codigoEol, CancellationToken cancellationToken = default);
    Task<IEnumerable<ComponenteCurricular>> ObterPorAnoAsync(int ano, CancellationToken cancellationToken = default);
    Task<bool> ExisteComCodigoEolAsync(int codigoEol, int? idIgnorar = null, CancellationToken cancellationToken = default);
}