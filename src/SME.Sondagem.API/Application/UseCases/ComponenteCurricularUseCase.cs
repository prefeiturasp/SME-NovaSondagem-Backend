using System.Collections.Generic;
using System.Threading.Tasks;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Application.UseCases
{
    public class ComponenteCurricularUseCase : IComponenteCurricularUseCase
    {
        private readonly IComponenteCurricularRepository componenteRepository;

        public ComponenteCurricularUseCase(IComponenteCurricularRepository componenteRepository)
        {
            this.componenteRepository = componenteRepository;
        }

        public async Task<IEnumerable<object>> ObterComponentesAsync()
        {
            return await componenteRepository.ObterTodosAsync();
        }
    }
}
