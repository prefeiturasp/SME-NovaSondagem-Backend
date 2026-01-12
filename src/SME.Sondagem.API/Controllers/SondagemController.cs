using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class SondagemController : ControllerBase
{
    private readonly ISondagemUseCase sondagemUseCase;
    private readonly ISondagemSalvarRespostasUseCase sondagemSalvarRespostasUseCase;

    public SondagemController(ISondagemUseCase sondagemUseCase, ISondagemSalvarRespostasUseCase sondagemSalvarRespostasUseCase)
    {
        this.sondagemUseCase = sondagemUseCase;
        this.sondagemSalvarRespostasUseCase = sondagemSalvarRespostasUseCase;
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
        try
        {
            return Ok(await sondagemSalvarRespostasUseCase.SalvarOuAtualizarSondagemAsync(dto));
        }
        catch (RegraNegocioException ex)
        {
            return StatusCode(ex.StatusCode, new { mensagem = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao salvar sondagem" });
        }
    }

}
