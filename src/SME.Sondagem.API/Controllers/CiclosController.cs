using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class CiclosController : ControllerBase
{
    private readonly ICicloUseCase cicloUseCase;

    public CiclosController(ICicloUseCase cicloUseCase)
    {
        this.cicloUseCase = cicloUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var resultado = await cicloUseCase.ObterCiclosAsync();
        return Ok(resultado);
    }
}
