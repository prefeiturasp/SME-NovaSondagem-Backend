using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.Interfaces.Ciclo;

public interface IAtualizarCicloUseCase
{
    Task<CicloDto?> ExecutarAsync(long id, CicloDto cicloDto, CancellationToken cancellationToken = default);
}