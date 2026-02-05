using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;

public interface IObterParametrosSondagemQuestionarioUseCase
{
    Task<IEnumerable<ParametroSondagemQuestionarioDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}