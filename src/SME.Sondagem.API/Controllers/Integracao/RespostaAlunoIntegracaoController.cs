using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;

namespace SME.Sondagem.API.Controllers.Integracao;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class RespostaAlunoIntegracaoController : ControllerBase
{
    private readonly IAtualizarAeeRespostaAlunoUseCase atualizarAeeRespostaAlunoUseCase;

    public RespostaAlunoIntegracaoController(IAtualizarAeeRespostaAlunoUseCase atualizarAeeRespostaAlunoUseCase)
    {
        this.atualizarAeeRespostaAlunoUseCase = atualizarAeeRespostaAlunoUseCase;
    }

    [HttpPatch("aee")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarAee([FromQuery] int ultimoId, CancellationToken cancellationToken)
    {
        var resultado = await atualizarAeeRespostaAlunoUseCase.ExecutarAsync(ultimoId, cancellationToken);
        return Ok(new { resultado.UltimoId, TotalAtualizados = resultado.TotalAtualizado });
    }
}
