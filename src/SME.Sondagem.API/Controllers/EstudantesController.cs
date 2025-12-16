using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Application.Interfaces;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EstudantesController : ControllerBase
    {
        private readonly IEstudantesUseCase estudantesUseCase;

        public EstudantesController(IEstudantesUseCase estudantesUseCase)
        {
            this.estudantesUseCase = estudantesUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resultado = await estudantesUseCase.ObterEstudantesAsync();
            return Ok(resultado);
        }
    }
}
