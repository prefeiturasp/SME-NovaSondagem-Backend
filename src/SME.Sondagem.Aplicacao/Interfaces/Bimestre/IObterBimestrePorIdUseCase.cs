using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.Interfaces.Bimestre;

public interface IObterBimestrePorIdUseCase
{
    Task<BimestreDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}