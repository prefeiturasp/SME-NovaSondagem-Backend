using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.Interfaces.Ciclo;

public interface IObterCiclosUseCase
{
    Task<IEnumerable<CicloDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}