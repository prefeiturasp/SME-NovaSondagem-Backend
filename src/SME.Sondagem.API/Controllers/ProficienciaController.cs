using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Application.Interfaces;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProficienciaController : ControllerBase
    {
        private readonly IProficienciaUseCase proficienciaUseCase;

        public ProficienciaController(IProficienciaUseCase proficienciaUseCase)
        {
            this.proficienciaUseCase = proficienciaUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resultado = await proficienciaUseCase.ObterProficienciasAsync();
            return Ok(resultado);
        }
    }
}
