using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IConsultaDeDresService
    {
        Task<IEnumerable<ObterDresSgpDto>> ObterDresSgpAsync(CancellationToken cancellationToken = default);
    }
}
