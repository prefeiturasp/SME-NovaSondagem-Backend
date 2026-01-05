using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class ComponenteCurricularController : ControllerBase
{
    private readonly IComponenteCurricularUseCase _useCase;
    private readonly ILogger<ComponenteCurricularController> _logger;

    #region LoggerMessage delegates

    private static readonly Action<ILogger, Exception?> LogListagemCancelada =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(3001, nameof(LogListagemCancelada)),
            "Requisição de listagem de componentes curriculares foi cancelada");

    private static readonly Action<ILogger, Exception?> LogErroListagem =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(3002, nameof(LogErroListagem)),
            "Erro ao listar componentes curriculares");

    private static readonly Action<ILogger, int, Exception?> LogObtencaoCancelada =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(3003, nameof(LogObtencaoCancelada)),
            "Requisição de obtenção foi cancelada para ID {Id}");

    private static readonly Action<ILogger, int, Exception?> LogErroObtencao =
        LoggerMessage.Define<int>(
            LogLevel.Error,
            new EventId(3004, nameof(LogErroObtencao)),
            "Erro ao obter componente curricular {Id}");

    private static readonly Action<ILogger, int, Exception?> LogObtencaoCodigoEolCancelada =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(3005, nameof(LogObtencaoCodigoEolCancelada)),
            "Requisição foi cancelada para código EOL {CodigoEol}");

    private static readonly Action<ILogger, int, Exception?> LogErroObtencaoCodigoEol =
        LoggerMessage.Define<int>(
            LogLevel.Error,
            new EventId(3006, nameof(LogErroObtencaoCodigoEol)),
            "Erro ao obter componente curricular por código EOL {CodigoEol}");

    private static readonly Action<ILogger, Exception?> LogCriacaoCancelada =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(3007, nameof(LogCriacaoCancelada)),
            "Requisição de criação de componente curricular foi cancelada");

    private static readonly Action<ILogger, Exception?> LogErroCriacao =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(3008, nameof(LogErroCriacao)),
            "Erro ao criar componente curricular");

    private static readonly Action<ILogger, int, Exception?> LogAtualizacaoCancelada =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(3009, nameof(LogAtualizacaoCancelada)),
            "Requisição de atualização foi cancelada para ID {Id}");

    private static readonly Action<ILogger, int, Exception?> LogErroAtualizacao =
        LoggerMessage.Define<int>(
            LogLevel.Error,
            new EventId(3010, nameof(LogErroAtualizacao)),
            "Erro ao atualizar componente curricular {Id}");

    private static readonly Action<ILogger, int, Exception?> LogExclusaoCancelada =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(3011, nameof(LogExclusaoCancelada)),
            "Requisição de exclusão foi cancelada para ID {Id}");

    private static readonly Action<ILogger, int, Exception?> LogErroExclusao =
        LoggerMessage.Define<int>(
            LogLevel.Error,
            new EventId(3012, nameof(LogErroExclusao)),
            "Erro ao excluir componente curricular {Id}");

    #endregion

    public ComponenteCurricularController(
        IComponenteCurricularUseCase useCase,
        ILogger<ComponenteCurricularController> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ComponenteCurricularDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var componentes = await _useCase.ListarAsync(cancellationToken);
            return Ok(componentes);
        }
        catch (OperationCanceledException)
        {
            LogListagemCancelada(_logger, null);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            LogErroListagem(_logger, ex);
            return StatusCode(500, new { mensagem = "Erro ao listar componentes curriculares" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ComponenteCurricularDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
    {
        try
        {
            var componente = await _useCase.ObterPorIdAsync(id, cancellationToken);

            if (componente == null)
                return NotFound(new { mensagem = $"Componente curricular com ID {id} não encontrado" });

            return Ok(componente);
        }
        catch (OperationCanceledException)
        {
            LogObtencaoCancelada(_logger, id, null);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            LogErroObtencao(_logger, id, ex);
            return StatusCode(500, new { mensagem = "Erro ao obter componente curricular" });
        }
    }

    [HttpGet("codigo-eol/{codigoEol}")]
    [ProducesResponseType(typeof(ComponenteCurricularDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorCodigoEol(int codigoEol, CancellationToken cancellationToken)
    {
        try
        {
            var componente = await _useCase.ObterPorCodigoEolAsync(codigoEol, cancellationToken);

            if (componente == null)
                return NotFound(new { mensagem = $"Componente curricular com código EOL {codigoEol} não encontrado" });

            return Ok(componente);
        }
        catch (OperationCanceledException)
        {
            LogObtencaoCodigoEolCancelada(_logger, codigoEol, null);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            LogErroObtencaoCodigoEol(_logger, codigoEol, ex);
            return StatusCode(500, new { mensagem = "Erro ao obter componente curricular" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ComponenteCurricularDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CriarComponenteCurricularDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var componente = await _useCase.CriarAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = componente.Id },
                componente);
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
            return StatusCode(500, new { mensagem = "Erro ao criar componente curricular" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ComponenteCurricularDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarComponenteCurricularDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var componente = await _useCase.AtualizarAsync(id, dto, cancellationToken);
            return Ok(componente);
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
            return StatusCode(500, new { mensagem = "Erro ao atualizar componente curricular" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _useCase.ExcluirAsync(id, cancellationToken);

            if (!resultado)
                return NotFound(new { mensagem = $"Componente curricular com ID {id} não encontrado" });

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
            return StatusCode(500, new { mensagem = "Erro ao excluir componente curricular" });
        }
    }
}
