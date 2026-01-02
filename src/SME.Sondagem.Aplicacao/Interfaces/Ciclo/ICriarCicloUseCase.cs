using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.Interfaces.Ciclo;

public interface ICriarCicloUseCase
{
    Task<long> ExecutarAsync(CicloDto cicloDto, CancellationToken cancellationToken = default);
}