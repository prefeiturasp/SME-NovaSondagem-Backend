using System.Collections.Generic;
using System.Threading.Tasks;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Application.UseCases
{
    public class SondagemUseCase : ISondagemUseCase
    {
        private readonly ISondagemRepository sondagemRepository;

        public SondagemUseCase(ISondagemRepository sondagemRepository)
        {
            this.sondagemRepository = sondagemRepository;
        }

        public async Task<IEnumerable<object>> ObterTodasSondagensAsync()
        {
            return await sondagemRepository.ObterTodosAsync();
        }
    }
}
