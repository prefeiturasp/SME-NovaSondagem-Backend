using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioBase<T> where T : EntidadeBase
{
    Task<IEnumerable<T>> ListarAsync(CancellationToken cancellationToken = default);
    Task<T?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task RemoverAsync(long id, CancellationToken cancellationToken = default);
    Task RemoverAsync(T entidade, CancellationToken cancellationToken = default);
    Task<long> RemoverLogico(long id, string coluna = null, CancellationToken cancellationToken = default);
    Task<bool> RemoverLogico(long[] ids, string coluna = null, CancellationToken cancellationToken = default);
    Task<long> SalvarAsync(T entidade, CancellationToken cancellationToken = default);
    Task<bool> RestaurarAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ListarTodosIncluindoExcluidosAsync(CancellationToken cancellationToken = default);
}