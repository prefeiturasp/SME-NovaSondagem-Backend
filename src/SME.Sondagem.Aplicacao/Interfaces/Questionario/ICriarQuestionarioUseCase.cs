using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface ICriarQuestionarioUseCase
{
    Task<long> ExecutarAsync(QuestionarioDto questionarioDto, CancellationToken cancellationToken = default);
}