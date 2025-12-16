using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.Sondagem.Infra.Repositories
{
    public interface ICicloRepository
    {
        Task<IEnumerable<object>> ObterTodosAsync();
        Task<object> ObterPorIdAsync(Guid id);
        Task InserirAsync(object entidade);
    }
}
