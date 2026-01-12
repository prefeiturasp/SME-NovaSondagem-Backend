using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.Interfaces.Bimestre;

public interface IAtualizarBimestreUseCase
{
    Task<BimestreDto?> ExecutarAsync(long id, BimestreDto bimestreDto, CancellationToken cancellationToken = default);
}