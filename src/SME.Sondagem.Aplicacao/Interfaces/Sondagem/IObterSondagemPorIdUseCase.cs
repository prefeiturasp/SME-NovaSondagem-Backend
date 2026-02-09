using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface IObterSondagemPorIdUseCase
{
    Task<SondagemDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}