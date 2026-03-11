using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Infra.Dtos;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

namespace SME.Sondagem.API.Controllers;

/// <summary>
/// Controller de integração para relatórios - USO EXCLUSIVO DO SERVIDOR DE RELATÓRIOS
/// </summary>
/// <remarks>
/// Este controller deve ser utilizado única e exclusivamente pelo servidor de relatórios.
/// Não deve ser consumido por outras aplicações ou serviços.
/// </remarks>
//[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/relatorio-integracao")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class RelatorioIntegracaoController : ControllerBase
{
    [HttpGet("sondagem-por-turma")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(QuestionarioSondagemRelatorioDto), 200)]
    public async Task<IActionResult> ObterRelatorioSondagemPorTurma([FromQuery] FiltroQuestionario filtro, [FromServices] IObterSondagemRelatorioPorTurmaUseCase obterRelatorioSondagemPorTurmaUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterRelatorioSondagemPorTurmaUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("sondagem-por-todas-turma")]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(MemoryStream), 200)]
    public async Task<IActionResult> ObterRelatorioSondagemPorTodasTurma([FromServices] IObterSondagemRelatorioPorTodasTurmaUseCase useCase, CancellationToken cancellationToken)
    {
       var resultado = await useCase.ObterSondagemRelatorio(cancellationToken);
        return File(resultado.Content, resultado.ContentType, resultado.FileName);
    }
}

