using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class ProficienciaController : ControllerBase
{
    private readonly IProficienciaUseCase proficienciaUseCase;

    public ProficienciaController(IProficienciaUseCase proficienciaUseCase)
    {
        this.proficienciaUseCase = proficienciaUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var resultado = await proficienciaUseCase.ObterProficienciasAsync();
        return Ok(resultado);
    }
}
