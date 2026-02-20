using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IObterQuestionarioSondagemUseCase
{
    public Task<object> ObterQuestionarioSondagem([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken);
}
