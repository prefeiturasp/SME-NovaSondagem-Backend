using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioComponenteCurricular : IRepositorioBase<ComponenteCurricular>
{
    Task<ComponenteCurricular?> ObterPorCodigoEolAsync(int codigoEol);
    Task<IEnumerable<ComponenteCurricular>> ObterPorAnoAsync(int ano);
    Task<bool> ExisteComCodigoEolAsync(int codigoEol, int? idIgnorar = null);
}