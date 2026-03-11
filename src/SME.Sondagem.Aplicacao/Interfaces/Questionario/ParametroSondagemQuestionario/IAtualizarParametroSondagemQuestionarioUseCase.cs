using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;

public interface IAtualizarParametroSondagemQuestionarioUseCase
{
    Task<ParametroSondagemQuestionarioDto?> ExecutarAsync(long id, ParametroSondagemQuestionarioDto parametroSondagemQuestionarioDto, CancellationToken cancellationToken = default);
}