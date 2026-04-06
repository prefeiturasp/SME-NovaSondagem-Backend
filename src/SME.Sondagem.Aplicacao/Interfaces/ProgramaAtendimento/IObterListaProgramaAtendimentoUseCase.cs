using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.ProgramaAtendimento
{
    public interface IObterListaProgramaAtendimentoUseCase
    {
        Task<IEnumerable<ItemMenuDto>> Executar(CancellationToken cancellationToken = default);
    }
}
