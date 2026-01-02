using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioCiclo
{
    Task<IEnumerable<Ciclo>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<Ciclo?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> CriarAsync(Ciclo ciclo, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(Ciclo ciclo, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default);
}
