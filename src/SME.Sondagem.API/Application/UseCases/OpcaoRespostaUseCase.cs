using System.Collections.Generic;
using System.Threading.Tasks;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Application.UseCases
{
    public class OpcaoRespostaUseCase : IOpcaoRespostaUseCase
    {
        private readonly IOpcaoRespostaRepository opcaoRespostaRepository;

        public OpcaoRespostaUseCase(IOpcaoRespostaRepository opcaoRespostaRepository)
        {
            this.opcaoRespostaRepository = opcaoRespostaRepository;
        }

        public async Task<IEnumerable<object>> ObterOpcoesRespostaAsync()
        {
            return await opcaoRespostaRepository.ObterTodosAsync();
        }
    }
}
