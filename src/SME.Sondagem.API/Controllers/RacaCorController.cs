using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Infra.Constantes.Autenticacao;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
    public class RacaCorController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromServices] IObterListaRacaCorUseCase obterListaRacaCorUseCase, CancellationToken cancellationToken)
        {
            var resultado = await obterListaRacaCorUseCase.Executar(cancellationToken);
            return Ok(resultado);
        }   
    }

}
