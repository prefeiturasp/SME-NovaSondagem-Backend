using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Application.Interfaces;

namespace SME.Sondagem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IAutenticacaoUseCase authUseCase;

        public AutenticacaoController(IAutenticacaoUseCase authUseCase)
        {
            this.authUseCase = authUseCase ?? throw new ArgumentNullException(nameof(authUseCase));
        }

        [HttpPost]
        public async Task<IActionResult> Autenticar([FromBody] string apiAToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(apiAToken))
                    return BadRequest("Token da API A é obrigatório.");

                var resultado = await authUseCase.Autenticar(apiAToken);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Erro interno ao processar a autenticação: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado durante a autenticação.");
            }
        }

    }
}
