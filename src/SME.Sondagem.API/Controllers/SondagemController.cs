using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class SondagemController : ControllerBase
{
    private readonly ISondagemUseCase sondagemUseCase;

    public SondagemController(ISondagemUseCase sondagemUseCase)
    {
        this.sondagemUseCase = sondagemUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var resultado = await sondagemUseCase.ObterTodasSondagensAsync();
        return Ok(resultado);
    }

    [HttpPost("salvar")]
    public async Task<IActionResult> SalvarSondagem([FromBody] SondagemSalvarDto dto)
    {
        var resultado = await sondagemUseCase.SalvarOuAtualizarSondagemAsync(dto);
        return Ok(resultado);
    }

}
