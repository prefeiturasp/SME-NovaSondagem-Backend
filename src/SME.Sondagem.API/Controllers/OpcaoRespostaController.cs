using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class OpcaoRespostaController : ControllerBase
{
    private readonly IOpcaoRespostaUseCase opcaoRespostaUseCase;

    public OpcaoRespostaController(IOpcaoRespostaUseCase opcaoRespostaUseCase)
    {
        this.opcaoRespostaUseCase = opcaoRespostaUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var resultado = await opcaoRespostaUseCase.ObterOpcoesRespostaAsync();
        return Ok(resultado);
    }
}
