using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Constantes.Autenticacao;
using SME.Sondagem.Application.Interfaces.Questionario;
using SME.Sondagem.Infra.Dtos;

namespace SME.Sondagem.API.Controllers;


[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class QuestionarioController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<IActionResult> GetAll([FromServices] IObterQuestionarioSondagemUseCase obterQuestionarioSondagemUseCase)
    {
        return Ok(await obterQuestionarioSondagemUseCase.ObterQuestionarioSondagem());
    }
}
