using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class RelatorioController : ControllerBase
{
    [HttpGet("sondagemPorTurma")]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(QuestionarioSondagemRelatorioDto), 200)]
    public async Task<IActionResult> ObterRelatorioSondagemPorTurma([FromQuery] FiltroQuestionario filtro, [FromServices] IObterSondagemRelatorioPorTurmaUseCase obterRelatorioSondagemPorTurmaUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterRelatorioSondagemPorTurmaUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }
}
