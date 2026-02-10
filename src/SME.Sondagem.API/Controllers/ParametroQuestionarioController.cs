using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class ParametroQuestionarioController : ControllerBase
{
    private readonly IObterParametroSondagemQuestionarioPorIdQuestionarioUseCase obterParametroSondagemQuestionarioPorIdQuestionarioUseCase;

    public ParametroQuestionarioController(
         IObterParametroSondagemQuestionarioPorIdQuestionarioUseCase obterParametroSondagemQuestionarioPorIdQuestionarioUseCase)
    {
        this.obterParametroSondagemQuestionarioPorIdQuestionarioUseCase = obterParametroSondagemQuestionarioPorIdQuestionarioUseCase;
    }

    [HttpGet("questionario/{idQuestionario}")]
    [ProducesResponseType(typeof(ParametroSondagemQuestionarioCompletoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdQuestionario(long idQuestionario, CancellationToken cancellationToken)
    {
        var resultado = await obterParametroSondagemQuestionarioPorIdQuestionarioUseCase.ExecutarAsync(idQuestionario, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, idQuestionario));

        return Ok(resultado);
    }
}
