using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IObterQuestionarioPorIdUseCase
{
    Task<QuestionarioDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}