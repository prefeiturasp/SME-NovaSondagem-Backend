using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IAtualizarQuestionarioUseCase
{
    Task<QuestionarioDto?> ExecutarAsync(long id, QuestionarioDto questionarioDto, CancellationToken cancellationToken = default);
}