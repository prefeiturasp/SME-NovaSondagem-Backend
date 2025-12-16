using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Infra.Repositories.Postgres
{
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
}
