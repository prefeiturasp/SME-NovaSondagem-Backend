using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioQuestionario
{
    Task<IEnumerable<Questionario>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<Questionario?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> CriarAsync(Questionario questionario, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(Questionario questionario, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default);
}
