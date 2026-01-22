using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Turma;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class TurmaController : ControllerBase
{
    [HttpGet("validar-turma")]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<IActionResult> ObterPermissaoTurma([FromQuery] int turmaId, [FromServices] IObterPermissaoTurmaUseCase obterPermissaoTurmaUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterPermissaoTurmaUseCase.ObterPermissaoTurma(turmaId, cancellationToken));
    }
}
