using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface ICriarSondagemUseCase
{
    Task<long> ExecutarAsync(SondagemDto sondagemDto, CancellationToken cancellationToken = default);
}