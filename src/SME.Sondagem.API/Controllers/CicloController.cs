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
    private readonly ILogger<CicloController> _logger;

    #region LoggerMessage delegates

    private static readonly Action<ILogger, Exception?> LogListagemCancelada =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(2001, nameof(LogListagemCancelada)),
            "Requisição de listagem de ciclos foi cancelada");

    private static readonly Action<ILogger, Exception?> LogErroListagem =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(2002, nameof(LogErroListagem)),
            "Erro ao listar ciclos");

    private static readonly Action<ILogger, long, Exception?> LogObtencaoCancelada =
        LoggerMessage.Define<long>(
            LogLevel.Information,
            new EventId(2003, nameof(LogObtencaoCancelada)),
            "Requisição de obtenção foi cancelada para ID {Id}");

    private static readonly Action<ILogger, long, Exception?> LogErroObtencao =
        LoggerMessage.Define<long>(
            LogLevel.Error,
            new EventId(2004, nameof(LogErroObtencao)),
            "Erro ao obter ciclo {Id}");

    private static readonly Action<ILogger, Exception?> LogCriacaoCancelada =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(2005, nameof(LogCriacaoCancelada)),
            "Requisição de criação de ciclo foi cancelada");

    private static readonly Action<ILogger, Exception?> LogErroCriacao =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(2006, nameof(LogErroCriacao)),
            "Erro ao criar ciclo");

    private static readonly Action<ILogger, long, Exception?> LogAtualizacaoCancelada =
        LoggerMessage.Define<long>(
            LogLevel.Information,
            new EventId(2007, nameof(LogAtualizacaoCancelada)),
            "Requisição de atualização foi cancelada para ID {Id}");

    private static readonly Action<ILogger, long, Exception?> LogErroAtualizacao =
        LoggerMessage.Define<long>(
            LogLevel.Error,
            new EventId(2008, nameof(LogErroAtualizacao)),
            "Erro ao atualizar ciclo {Id}");

    private static readonly Action<ILogger, long, Exception?> LogExclusaoCancelada =
        LoggerMessage.Define<long>(
            LogLevel.Information,
            new EventId(2009, nameof(LogExclusaoCancelada)),
            "Requisição de exclusão foi cancelada para ID {Id}");

    private static readonly Action<ILogger, long, Exception?> LogErroExclusao =
        LoggerMessage.Define<long>(
            LogLevel.Error,
            new EventId(2010, nameof(LogErroExclusao)),
            "Erro ao excluir ciclo {Id}");

    #endregion

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
            LogListagemCancelada(_logger, null);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            LogErroListagem(_logger, ex);
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
            LogObtencaoCancelada(_logger, id, null);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            LogErroObtencao(_logger, id, ex);
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
            LogCriacaoCancelada(_logger, null);
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
        catch (Exception ex)
        {
            LogErroCriacao(_logger, ex);
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
            LogAtualizacaoCancelada(_logger, id, null);
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
        catch (Exception ex)
        {
            LogErroAtualizacao(_logger, id, ex);
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
            LogExclusaoCancelada(_logger, id, null);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            LogErroExclusao(_logger, id, ex);
            return StatusCode(500, new { mensagem = "Erro ao excluir ciclo" });
        }
    }
}
