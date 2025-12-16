using System.Collections.Generic;
using System.Threading.Tasks;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Application.UseCases
{
    public class ProficienciaUseCase : IProficienciaUseCase
    {
        private readonly IProficienciaRepository proficienciaRepository;

        public ProficienciaUseCase(IProficienciaRepository proficienciaRepository)
        {
            this.proficienciaRepository = proficienciaRepository;
        }

        public async Task<IEnumerable<object>> ObterProficienciasAsync()
        {
            return await proficienciaRepository.ObterTodosAsync();
        }
    }
}
