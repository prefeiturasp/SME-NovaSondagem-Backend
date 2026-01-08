using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IObterQuestionarioSondagemUseCase
{
    public Task<Infra.Dtos.Questionario.QuestionarioSondagemDto> ObterQuestionarioSondagem([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken);
}
