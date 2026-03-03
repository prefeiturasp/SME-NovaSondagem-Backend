using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;

public interface IObterParametroSondagemPorIdUseCase
{
    Task<ParametroSondagemDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}