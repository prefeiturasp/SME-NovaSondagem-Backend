using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.GeneroSexo;
using SME.Sondagem.Infrastructure.Dtos;
using Microsoft.AspNetCore.Authorization;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
    public class GeneroSexoController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemMenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromServices] IObterListaGeneroSexoUseCase useCase, CancellationToken cancellationToken)
        {
            var resultado = await useCase.Executar(cancellationToken);
            return Ok(resultado);
        }
    }
}
