using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;

public interface ICriarParametroSondagemQuestionarioUseCase
{
    Task<long> ExecutarAsync(ParametroSondagemQuestionarioDto parametroSondagemQuestionarioDto, CancellationToken cancellationToken = default);
}