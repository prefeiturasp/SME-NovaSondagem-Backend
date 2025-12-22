using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Constantes.Autenticacao;
using SME.Sondagem.Application.Interfaces;

namespace SME.Sondagem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
    public class QuestaoController : ControllerBase
    {
        private readonly IQuestaoUseCase questaoUseCase;

        public QuestaoController(IQuestaoUseCase questaoUseCase)
        {
            this.questaoUseCase = questaoUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resultado = await questaoUseCase.ObterQuestoesAsync();
            return Ok(resultado);
        }
    }
}
