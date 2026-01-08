using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class QuestionarioController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(IEnumerable<QuestionarioDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] FiltroQuestionario filtro, [FromServices] IObterQuestionarioSondagemUseCase obterQuestionarioSondagemUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterQuestionarioSondagemUseCase.ObterQuestionarioSondagem(filtro, cancellationToken));
    }
}
