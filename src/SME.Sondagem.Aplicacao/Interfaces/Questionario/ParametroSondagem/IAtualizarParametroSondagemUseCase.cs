using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;

public interface IAtualizarParametroSondagemUseCase
{
    Task<ParametroSondagemDto?> ExecutarAsync(long id, ParametroSondagemDto parametroSondagemDto, CancellationToken cancellationToken = default);
}