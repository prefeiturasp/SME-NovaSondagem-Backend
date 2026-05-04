using SME.Sondagem.Aplicacao.Interfaces.RacaCor;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.UseCases.RacaCor
{
    public class ObterListaRacaCorUseCase : IObterListaRacaCorUseCase
    {
        private readonly IRepositorioRacaCor _repositorioRacaCor;

        public ObterListaRacaCorUseCase(IRepositorioRacaCor repositorioRacaCor)
        {
            _repositorioRacaCor = repositorioRacaCor ?? throw new ArgumentNullException(nameof(repositorioRacaCor));
        }

        public async Task<IEnumerable<ItemMenuDto>> Executar(CancellationToken cancellationToken = default)
        {
            var buscarNaBase = (await _repositorioRacaCor.ListarAsync(cancellationToken)).Where(x => !x.Excluido);
            if(!buscarNaBase.Any())
                return [];

            return buscarNaBase.Select(x => new ItemMenuDto(x.Id, x.Descricao));
        }
    }
}
