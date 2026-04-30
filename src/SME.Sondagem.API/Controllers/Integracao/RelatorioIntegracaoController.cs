using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Infra.Dtos;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.API.Controllers;

/// <summary>
/// Controller de integração para relatórios - USO EXCLUSIVO DO SERVIDOR DE RELATÓRIOS
/// </summary>
/// <remarks>
/// Este controller deve ser utilizado única e exclusivamente pelo servidor de relatórios.
/// Não deve ser consumido por outras aplicações ou serviços.
/// </remarks>
[Route("api/relatorio-integracao")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[ChaveIntegracaoApiAttribute]
public class RelatorioIntegracaoController : ControllerBase
{
    [HttpGet("sondagem-por-turma")]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(QuestionarioSondagemRelatorioDto), 200)]
    public async Task<IActionResult> ObterRelatorioSondagemPorTurma([FromQuery] FiltroQuestionario filtro, [FromServices] IObterSondagemRelatorioPorTurmaUseCase obterRelatorioSondagemPorTurmaUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterRelatorioSondagemPorTurmaUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("sondagem-por-todas-turma-lp")]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(MemoryStream), 200)]
    public async Task<IActionResult> ObterRelatorioSondagemPorTodasTurma([FromServices] IObterSondagemRelatorioPorTodasTurmaUseCase useCase, CancellationToken cancellationToken)
    {
       var resultado = await useCase.ObterSondagemRelatorio(cancellationToken);
        return File(resultado.Content, resultado.ContentType, resultado.FileName);
    }

    [HttpGet("proficiencia/{proficienciaId}")]
    [ProducesResponseType(typeof(ProficienciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int proficienciaId, [FromServices] IObterProficienciaPorIdUseCase useCase, CancellationToken cancellationToken)
    {
            var resultado = await useCase.ExecutarAsync(proficienciaId, cancellationToken);
            return Ok(resultado);
    }

    [HttpGet("consolidado/raca")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorRaca([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoRacaUseCase obterSondagemRelatorioConsolidadoUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/genero")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorGenero([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoGeneroUseCase obterSondagemRelatorioConsolidadoGeneroUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoGeneroUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/raca-genero")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorRacaGenero([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoRacaGeneroUseCase obterSondagemRelatorioConsolidadoRacaGeneroUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoRacaGeneroUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/ano")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorAno([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoAnoUseCase obterSondagemRelatorioConsolidadoAnoUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoAnoUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("consolidado/bimestre")]
    public async Task<IActionResult> ObterRelatorioSondagemConsolidadoPorBimestre([FromQuery] FiltroConsolidadoDto filtro, [FromServices] IObterSondagemRelatorioConsolidadoBimestreUseCase obterSondagemRelatorioConsolidadoBimestreUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterSondagemRelatorioConsolidadoBimestreUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }
}

