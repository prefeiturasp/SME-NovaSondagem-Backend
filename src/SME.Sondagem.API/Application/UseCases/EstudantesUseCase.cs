using System.Collections.Generic;
using System.Threading.Tasks;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Application.UseCases
{
    public class EstudantesUseCase : IEstudantesUseCase
    {
        private readonly IEstudantesRepository estudantesRepository;

        public EstudantesUseCase(IEstudantesRepository estudantesRepository)
        {
            this.estudantesRepository = estudantesRepository;
        }

        public async Task<IEnumerable<object>> ObterEstudantesAsync()
        {
            return await estudantesRepository.ObterTodosAsync();
        }
    }
}
