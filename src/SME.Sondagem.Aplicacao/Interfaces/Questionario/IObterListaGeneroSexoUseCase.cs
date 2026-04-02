using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario
{
    public interface IObterListaGeneroSexoUseCase 
    {
        Task<IEnumerable<ItemMenuDto>> Executar(CancellationToken cancellationToken = default);
    }
}
