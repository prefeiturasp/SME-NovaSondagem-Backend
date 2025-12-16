using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Application.Interfaces;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CicloController : ControllerBase
    {
        private readonly ICicloUseCase cicloUseCase;

        public CicloController(ICicloUseCase cicloUseCase)
        {
            this.cicloUseCase = cicloUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resultado = await cicloUseCase.ObterCiclosAsync();
            return Ok(resultado);
        }
    }
}
