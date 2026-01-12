using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.Interfaces.Bimestre;

public interface IObterBimestresUseCase
{
    Task<IEnumerable<BimestreDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}