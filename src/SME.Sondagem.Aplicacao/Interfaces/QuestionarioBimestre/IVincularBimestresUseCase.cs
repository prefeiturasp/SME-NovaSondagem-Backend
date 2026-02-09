using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

namespace SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;

public interface IVincularBimestresUseCase
{
    Task<bool> ExecutarAsync(VincularBimestresDto dto, CancellationToken cancellationToken = default);
    Task<bool> ExecutarAtualizacaoAsync(AtualizarVinculosBimestresDto dto, CancellationToken cancellationToken = default);
}