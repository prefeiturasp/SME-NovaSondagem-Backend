using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class OpcoesRespostasController : ControllerBase
{
    private readonly IOpcaoRespostaUseCase opcaoRespostaUseCase;

    public OpcoesRespostasController(IOpcaoRespostaUseCase opcaoRespostaUseCase)
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
