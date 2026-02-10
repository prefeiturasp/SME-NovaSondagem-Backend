using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;

public interface ICriarParametroSondagemUseCase
{
    Task<long> ExecutarAsync(ParametroSondagemDto parametroSondagemDto, CancellationToken cancellationToken = default);
}