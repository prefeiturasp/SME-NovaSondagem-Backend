using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.RacaCor;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
    public class RacaCorController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemMenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromServices] IObterListaRacaCorUseCase obterListaRacaCorUseCase, CancellationToken cancellationToken)
        {
            var resultado = await obterListaRacaCorUseCase.Executar(cancellationToken);
            return Ok(resultado);
        }   
    }

}
