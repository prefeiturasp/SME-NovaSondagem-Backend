using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;

public interface IObterParametroSondagemQuestionarioPorIdUseCase
{
    Task<ParametroSondagemQuestionarioDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}