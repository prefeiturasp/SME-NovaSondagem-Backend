using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Application.Interfaces;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SondagemController : ControllerBase
    {
        private readonly ISondagemUseCase sondagemUseCase;

        public SondagemController(ISondagemUseCase sondagemUseCase)
        {
            this.sondagemUseCase = sondagemUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resultado = await sondagemUseCase.ObterTodasSondagensAsync();
            return Ok(resultado);
        }
    }
}
