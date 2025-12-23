namespace SME.Sondagem.Dados.Interfaces;

public interface ICicloRepository
{
    Task<IEnumerable<object>> ObterTodosAsync();
    Task<object> ObterPorIdAsync(Guid id);
    Task InserirAsync(object entidade);
}
