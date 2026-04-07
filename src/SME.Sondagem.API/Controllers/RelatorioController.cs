using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class RelatorioController : ControllerBase
{
    [HttpGet("sondagem-por-turma")]
    public async Task<IActionResult> ObterRelatorioSondagemPorTurma([FromQuery] FiltroQuestionario filtro, [FromServices] IObterSondagemRelatorioPorTurmaUseCase obterRelatorioSondagemPorTurmaUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterRelatorioSondagemPorTurmaUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("sondagem-por-turma/exportar")]
    public async Task<IActionResult> ExportarRelatorioSondagemPorTurma([FromQuery] FiltroRelatorio filtro, [FromServices] IExportarSondagemRelatorioPorTurmaUseCase exportarSondagemRelatorioPorTurmaUseCase, CancellationToken cancellationToken)
    {
        await exportarSondagemRelatorioPorTurmaUseCase.ExportarSondagemRelatorio(filtro, cancellationToken);
        return Ok();
    }

    [HttpGet("consoliado/raca")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidado([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoRacaUseCase obterSondagemRelatorioConsolidadoUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

}
