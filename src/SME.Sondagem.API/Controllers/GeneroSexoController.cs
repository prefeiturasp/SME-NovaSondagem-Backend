using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.GeneroSexo;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos;

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
