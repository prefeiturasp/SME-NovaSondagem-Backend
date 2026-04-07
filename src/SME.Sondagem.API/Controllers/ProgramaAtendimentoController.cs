using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.ProgramaAtendimento;
using SME.Sondagem.Infrastructure.Dtos;
using Microsoft.AspNetCore.Authorization;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
    public class ProgramaAtendimentoController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemMenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromServices] IObterListaProgramaAtendimentoUseCase useCase, CancellationToken cancellationToken)
        {
            var resultado = await useCase.Executar(cancellationToken);
            return Ok(resultado);
        }
    }
}
