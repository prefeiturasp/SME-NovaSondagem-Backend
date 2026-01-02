using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.Interfaces.Ciclo;

public interface IObterCicloPorIdUseCase
{
    Task<CicloDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}