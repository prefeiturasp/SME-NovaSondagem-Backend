using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface IObterSondagensUseCase
{
    Task<IEnumerable<SondagemDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}