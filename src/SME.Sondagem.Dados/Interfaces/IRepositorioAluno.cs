namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioAluno
{
    Task<IEnumerable<object>> ObterTodosAsync();
    Task<object> ObterPorIdAsync(Guid id);
    Task InserirAsync(object entidade);
}
