using System.Collections.Generic;
using System.Threading.Tasks;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Application.UseCases
{
    public class CicloUseCase : ICicloUseCase
    {
        private readonly ICicloRepository cicloRepository;

        public CicloUseCase(ICicloRepository cicloRepository)
        {
            this.cicloRepository = cicloRepository;
        }

        public async Task<IEnumerable<object>> ObterCiclosAsync()
        {
            return await cicloRepository.ObterTodosAsync();
        }
    }
}
