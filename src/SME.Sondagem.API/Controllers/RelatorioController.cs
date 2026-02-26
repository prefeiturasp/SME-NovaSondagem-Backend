using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class RelatorioController : ControllerBase
{
    [HttpGet("sondagem-por-turma")]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(QuestionarioSondagemRelatorioDto), 200)]
    public async Task<IActionResult> ObterRelatorioSondagemPorTurma([FromQuery] FiltroQuestionario filtro, [FromServices] IObterSondagemRelatorioPorTurmaUseCase obterRelatorioSondagemPorTurmaUseCase, CancellationToken cancellationToken)
    {
        return Ok(await obterRelatorioSondagemPorTurmaUseCase.ObterSondagemRelatorio(filtro, cancellationToken));
    }

    [HttpGet("sondagem-por-turma/exportar")]
    [ProducesResponseType(typeof(RetornoBaseDto), 500)]
    [ProducesResponseType(typeof(QuestionarioSondagemRelatorioDto), 200)]
    public async Task<IActionResult> ExportarRelatorioSondagemPorTurma([FromQuery] FiltroRelatorio filtro, [FromServices] IExportarSondagemRelatorioPorTurmaUseCase exportarSondagemRelatorioPorTurmaUseCase, CancellationToken cancellationToken)
    {
        await exportarSondagemRelatorioPorTurmaUseCase.ExportarSondagemRelatorio(filtro, cancellationToken);
        return Ok();
    }
}
