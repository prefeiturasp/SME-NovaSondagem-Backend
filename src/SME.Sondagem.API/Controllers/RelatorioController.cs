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

    [HttpGet("consolidado/raca")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorRaca([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoRacaUseCase obterSondagemRelatorioConsolidadoUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/raca/exportar")]
    public async Task<IActionResult> ExportarRelatorioSondagemConsolidadoPorRaca([FromQuery] FiltroRelatorioConsolidado filtro, [FromServices] IExportarSondagemRelatorioConsolidadoRacaUseCase exportarSondagemRelatorioConsolidadoRacaUseCase, CancellationToken cancellationToken)
    {
        await exportarSondagemRelatorioConsolidadoRacaUseCase.Exportar(filtro, cancellationToken);
        return Ok();
    }

    [HttpGet("consolidado/genero")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorGenero([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoGeneroUseCase obterSondagemRelatorioConsolidadoGeneroUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoGeneroUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/genero/exportar")]
    public async Task<IActionResult> ExportarRelatorioSondagemConsolidadoPorGenero([FromQuery] FiltroRelatorioConsolidado filtro, [FromServices] IExportarSondagemRelatorioConsolidadoGeneroUseCase exportarSondagemRelatorioConsolidadoGeneroUseCase, CancellationToken cancellationToken)
    {
        await exportarSondagemRelatorioConsolidadoGeneroUseCase.Exportar(filtro, cancellationToken);
        return Ok();
    }

    [HttpGet("consolidado/raca-genero")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorRacaGenero([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoRacaGeneroUseCase obterSondagemRelatorioConsolidadoRacaGeneroUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoRacaGeneroUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/raca-genero/exportar")]
    public async Task<IActionResult> ExportarRelatorioSondagemConsolidadoPorRacaGenero([FromQuery] FiltroRelatorioConsolidado filtro, [FromServices] IExportarSondagemRelatorioConsolidadoRacaGeneroUseCase exportarSondagemRelatorioConsolidadoRacaGeneroUseCase, CancellationToken cancellationToken)
    {
        await exportarSondagemRelatorioConsolidadoRacaGeneroUseCase.Exportar(filtro, cancellationToken);
        return Ok();
    }

    [HttpGet("consolidado/ano")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorAno([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoAnoUseCase obterSondagemRelatorioConsolidadoAnoUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoAnoUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/ano/exportar")]
    public async Task<IActionResult> ExportarRelatorioSondagemConsolidadoPorAno([FromQuery] FiltroRelatorioConsolidado filtro, [FromServices] IExportarSondagemRelatorioConsolidadoAnoUseCase exportarSondagemRelatorioConsolidadoAnoUseCase, CancellationToken cancellationToken)
    {
        await exportarSondagemRelatorioConsolidadoAnoUseCase.Exportar(filtro, cancellationToken);
        return Ok();
    }

    [HttpGet("consolidado/bimestre")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorBimestre([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoBimestreUseCase obterSondagemRelatorioConsolidadoBimestreUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoBimestreUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/bimestre/exportar")]
    public async Task<IActionResult> ExportarRelatorioSondagemConsolidadoPorBimestre([FromQuery] FiltroRelatorioConsolidado filtro, [FromServices] IExportarSondagemRelatorioConsolidadoBimestreUseCase exportarSondagemRelatorioConsolidadoBimestreUseCase, CancellationToken cancellationToken)
    {
        await exportarSondagemRelatorioConsolidadoBimestreUseCase.Exportar(filtro, cancellationToken);
        return Ok();
    }
}
