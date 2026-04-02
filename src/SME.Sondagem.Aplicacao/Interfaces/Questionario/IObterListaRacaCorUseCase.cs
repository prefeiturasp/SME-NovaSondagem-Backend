using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario
{
    public interface IObterListaRacaCorUseCase
    {
        Task<IEnumerable<ItemMenuDto>> Executar(CancellationToken cancellationToken = default);
    }
}
