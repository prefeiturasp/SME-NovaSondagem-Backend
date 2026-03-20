using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IUeComDreEolService
    {
        public Task<IEnumerable<UeComDreEolDto>> ObterUesComDrePorCodigosUes(IEnumerable<string> codigosUes, CancellationToken cancellationToken = default);
    }
}
