using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioAluno : IRepositorioAluno
{
    public RepositorioAluno()
    {
        
    }

    public Task InserirAsync(object entidade)
    {
        throw new NotImplementedException();
    }

    public Task<object> ObterPorIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<object>> ObterTodosAsync()
    {
        throw new NotImplementedException();
    }
}