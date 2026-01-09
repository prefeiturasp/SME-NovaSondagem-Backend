using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.Interfaces.Bimestre;

public interface ICriarBimestreUseCase
{
    Task<long> ExecutarAsync(BimestreDto bimestreDto, CancellationToken cancellationToken = default);
}