using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Aluno;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.API.Controllers;

[ExcludeFromCodeCoverage]
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class AlunoController : ControllerBase
{
    private readonly IAlunoUseCase alunosUseCase;

    public AlunoController(IAlunoUseCase alunosUseCase)
    {
        this.alunosUseCase = alunosUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var resultado = await alunosUseCase.ObterAlunosAsync();
        return Ok(resultado);
    }
}
