namespace SME.Sondagem.Dados.Interfaces;

public interface IEstudantesRepository
{
    Task<IEnumerable<object>> ObterTodosAsync();
    Task<object> ObterPorIdAsync(Guid id);
    Task InserirAsync(object entidade);
}
