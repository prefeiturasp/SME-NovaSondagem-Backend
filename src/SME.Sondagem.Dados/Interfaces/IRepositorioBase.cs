using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioBase<T> where T : EntidadeBase
{
    Task<IEnumerable<T>> ListarAsync();
    Task<T?> ObterPorIdAsync(long id);
    Task RemoverAsync(long id);
    Task RemoverAsync(T entidade);
    Task<long> RemoverLogico(long id, string coluna = null);
    Task<bool> RemoverLogico(long[] ids, string coluna = null);
    Task<long> SalvarAsync(T entidade);
    Task<bool> RestaurarAsync(long id);
    Task<IEnumerable<T>> ListarTodosIncluindoExcluidosAsync();
}