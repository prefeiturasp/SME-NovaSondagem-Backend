using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class SondagemController : ControllerBase
{
    private readonly ISondagemSalvarRespostasUseCase sondagemSalvarRespostasUseCase;

    public SondagemController(ISondagemSalvarRespostasUseCase sondagemSalvarRespostasUseCase)
    {
        this.sondagemSalvarRespostasUseCase = sondagemSalvarRespostasUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] SondagemSalvarDto dto)
    {
        try
        {
            var result = await sondagemSalvarRespostasUseCase.SalvarOuAtualizarSondagemAsync(dto);
            return Ok(result);
        }
        catch (NegocioException ex)
        {
            return StatusCode(ex.StatusCode, new { mensagem = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao salvar sondagem" });
        }
    }

}
