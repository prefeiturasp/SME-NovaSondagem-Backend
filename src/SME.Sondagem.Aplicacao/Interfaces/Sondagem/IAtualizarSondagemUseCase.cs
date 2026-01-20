using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface IAtualizarSondagemUseCase
{
    Task<SondagemDto?> ExecutarAsync(long id, SondagemDto sondagemDto, CancellationToken cancellationToken = default);
}