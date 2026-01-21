using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IObterQuestionariosUseCase
{
    Task<IEnumerable<QuestionarioDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}