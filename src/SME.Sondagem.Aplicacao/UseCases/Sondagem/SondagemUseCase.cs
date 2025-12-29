using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem
{
    public class SondagemUseCase : ISondagemUseCase
    {
        private readonly IRepositorioSondagem sondagemRepository;

        public SondagemUseCase(IRepositorioSondagem sondagemRepository)
        {
            this.sondagemRepository = sondagemRepository;
        }

        public async Task<IEnumerable<object>> ObterTodasSondagensAsync()
        {
            return await sondagemRepository.ObterTodosAsync();
        }
    }
}
