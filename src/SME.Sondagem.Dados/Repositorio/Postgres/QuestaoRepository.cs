using Microsoft.Extensions.Configuration;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class QuestaoRepository : IQuestaoRepository
{
    private readonly IConfiguration configuration;

    public QuestaoRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
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
