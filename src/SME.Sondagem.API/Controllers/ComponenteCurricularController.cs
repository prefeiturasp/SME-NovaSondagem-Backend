using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class ComponenteCurricularController : ControllerBase
{
    private readonly IComponenteCurricularUseCase componenteUseCase;

    public ComponenteCurricularController(IComponenteCurricularUseCase componenteUseCase)
    {
        this.componenteUseCase = componenteUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var resultado = await componenteUseCase.ObterComponentesAsync();
        return Ok(resultado);
    }
}
