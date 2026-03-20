using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Infra.Dtos;

namespace SME.Sondagem.API.Controllers.Integracao
{
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
        [HttpGet("sondagem-por-todas-turma-lp")]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        [ProducesResponseType(typeof(MemoryStream), 200)]
        public async Task<IActionResult> ObterRelatorioSondagemPorTodasTurma([FromServices] IObterSondagemRelatorioPorTodasTurmaUseCase useCase, CancellationToken cancellationToken)
        {
            var resultado = await useCase.ObterSondagemRelatorio(cancellationToken);
            return File(resultado.Content, resultado.ContentType, resultado.FileName);
        }
    }
}
