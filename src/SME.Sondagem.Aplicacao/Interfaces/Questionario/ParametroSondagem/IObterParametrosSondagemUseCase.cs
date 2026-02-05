using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;

public interface IObterParametrosSondagemUseCase
{
    Task<IEnumerable<ParametroSondagemDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}