using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;

public interface IObterParametroSondagemQuestionarioPorIdQuestionarioUseCase
{
    Task<IEnumerable<ParametroSondagemQuestionarioCompletoDto>> ExecutarAsync(long idQuestionario, CancellationToken cancellationToken = default);
}