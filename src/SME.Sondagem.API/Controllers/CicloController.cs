using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class CicloController : ControllerBase
{
    private readonly ICriarCicloUseCase _criarCicloUseCase;
    private readonly IAtualizarCicloUseCase _atualizarCicloUseCase;
    private readonly IExcluirCicloUseCase _excluirCicloUseCase;
    private readonly IObterCicloPorIdUseCase _obterCicloPorIdUseCase;
    private readonly IObterCiclosUseCase _obterCicloUseCase;

    public CicloController(
        ICriarCicloUseCase criarCicloUseCase,
        IAtualizarCicloUseCase atualizarCicloUseCase,
        IExcluirCicloUseCase excluirCicloUseCase,
        IObterCicloPorIdUseCase obterCicloPorIdUseCase,
        IObterCiclosUseCase obterCicloUseCase)
    {
        _criarCicloUseCase = criarCicloUseCase;
        _atualizarCicloUseCase = atualizarCicloUseCase;
        _excluirCicloUseCase = excluirCicloUseCase;
        _obterCicloPorIdUseCase = obterCicloPorIdUseCase;
        _obterCicloUseCase = obterCicloUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CicloDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var ciclos = await _obterCicloUseCase.ExecutarAsync(cancellationToken);
            return Ok(ciclos);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao listar ciclos" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CicloDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(long id, CancellationToken cancellationToken)
    {
        try
        {
            var ciclo = await _obterCicloPorIdUseCase.ExecutarAsync(id, cancellationToken);

            if (ciclo == null)
                return NotFound(new { mensagem = $"Ciclo com ID {id} não encontrado" });

            return Ok(ciclo);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao obter ciclo" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CicloDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _criarCicloUseCase.ExecutarAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id },
                new { id });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (FluentValidation.ValidationException ex)
        {
            var erros = ex.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage });
            return BadRequest(new { mensagem = "Erro de validação", erros });
        }
        catch (RegraNegocioException ex)
        {
            return StatusCode(ex.StatusCode, new { mensagem = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao criar ciclo" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(long id, [FromBody] CicloDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var ciclo = await _atualizarCicloUseCase.ExecutarAsync(id, dto, cancellationToken);

            if (ciclo == null)
                return NotFound(new { mensagem = $"Ciclo com ID {id} não encontrado" });

            return Ok(ciclo);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (FluentValidation.ValidationException ex)
        {
            var erros = ex.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage });
            return BadRequest(new { mensagem = "Erro de validação", erros });
        }
        catch (RegraNegocioException ex)
        {
            return StatusCode(ex.StatusCode, new { mensagem = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao atualizar ciclo" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(long id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _excluirCicloUseCase.ExecutarAsync(id, cancellationToken);

            if (!resultado)
                return NotFound(new { mensagem = $"Ciclo com ID {id} não encontrado" });

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao excluir ciclo" });
        }
    }
}
