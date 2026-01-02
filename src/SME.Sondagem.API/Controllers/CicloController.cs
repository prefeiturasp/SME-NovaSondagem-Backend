using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly ILogger<CicloController> _logger;

    public CicloController(
        ICriarCicloUseCase criarCicloUseCase,
        IAtualizarCicloUseCase atualizarCicloUseCase,
        IExcluirCicloUseCase excluirCicloUseCase,
        IObterCicloPorIdUseCase obterCicloPorIdUseCase,
        IObterCiclosUseCase obterCicloUseCase,
        ILogger<CicloController> logger)
    {
        _criarCicloUseCase = criarCicloUseCase;
        _atualizarCicloUseCase = atualizarCicloUseCase;
        _excluirCicloUseCase = excluirCicloUseCase;
        _obterCicloPorIdUseCase = obterCicloPorIdUseCase;
        _obterCicloUseCase = obterCicloUseCase;
        _logger = logger;
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
            _logger.LogInformation("Requisição de listagem foi cancelada");
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar ciclos");
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
            _logger.LogInformation("Requisição de obtenção foi cancelada para ID {Id}", id);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ciclo {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao obter ciclo" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] CicloDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _criarCicloUseCase.ExecutarAsync(dto, cancellationToken);
            return CreatedAtAction(
                nameof(ObterPorId),
                new { id },
                new { id }
            );
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Requisição de criação foi cancelada");
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (FluentValidation.ValidationException ex)
        {
            var erros = ex.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage });
            return BadRequest(new { mensagem = "Erro de validação", erros });
        }
        catch (NegocioException ex)
        {
            return StatusCode(ex.StatusCode, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar ciclo");
            return StatusCode(500, new { mensagem = "Erro ao criar ciclo" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CicloDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
            _logger.LogInformation("Requisição de atualização foi cancelada para ID {Id}", id);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (FluentValidation.ValidationException ex)
        {
            var erros = ex.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage });
            return BadRequest(new { mensagem = "Erro de validação", erros });
        }
        catch (NegocioException ex)
        {
            return StatusCode(ex.StatusCode, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar ciclo {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao atualizar ciclo" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            _logger.LogInformation("Requisição de exclusão foi cancelada para ID {Id}", id);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir ciclo {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao excluir ciclo" });
        }
    }
}
