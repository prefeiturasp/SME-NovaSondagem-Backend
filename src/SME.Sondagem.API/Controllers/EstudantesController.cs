using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Estudantes;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class EstudantesController : ControllerBase
{
    private readonly IEstudantesUseCase estudantesUseCase;

    public EstudantesController(IEstudantesUseCase estudantesUseCase)
    {
        this.estudantesUseCase = estudantesUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var resultado = await estudantesUseCase.ObterEstudantesAsync();
        return Ok(resultado);
    }
}
