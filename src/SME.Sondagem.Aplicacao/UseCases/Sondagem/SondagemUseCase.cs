
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem
{
    public class SondagemUseCase : ISondagemUseCase
    {
        private readonly IRepositorioSondagem _sondagemRepository;

        public SondagemUseCase(
            IRepositorioSondagem sondagemRepository)
        {
            _sondagemRepository = sondagemRepository;
        }

        public async Task<IEnumerable<object>> ObterTodasSondagensAsync()
        {
            return await _sondagemRepository.ObterTodosAsync();
        }
    }
}
