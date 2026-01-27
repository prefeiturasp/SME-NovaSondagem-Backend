using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

namespace SME.Sondagem.API.Controllers.Integracao;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class QuestionarioBimestreIntegracaoController : ControllerBase
{
    private readonly IVincularBimestresUseCase _vincularBimestresUseCase;
    private readonly IObterQuestionariosBimestresUseCase _obterTodosUseCase;
    private readonly IObterBimestresPorQuestionarioUseCase _obterPorQuestionarioUseCase;
    private readonly IExcluirVinculosPorQuestionarioUseCase _excluirPorQuestionarioUseCase;
    private readonly ILogger<QuestionarioBimestreIntegracaoController> _logger;

    public QuestionarioBimestreIntegracaoController(
        IVincularBimestresUseCase vincularBimestresUseCase,
        IObterQuestionariosBimestresUseCase obterTodosUseCase,
        IObterBimestresPorQuestionarioUseCase obterPorQuestionarioUseCase,
        IExcluirVinculosPorQuestionarioUseCase excluirPorQuestionarioUseCase,
        ILogger<QuestionarioBimestreIntegracaoController> logger)
    {
        _vincularBimestresUseCase = vincularBimestresUseCase;
        _obterTodosUseCase = obterTodosUseCase;
        _obterPorQuestionarioUseCase = obterPorQuestionarioUseCase;
        _excluirPorQuestionarioUseCase = excluirPorQuestionarioUseCase;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestionarioBimestreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Iniciando listagem de vínculos de questionário e bimestre");

            var vinculos = await _obterTodosUseCase.ExecutarAsync(cancellationToken);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Listagem concluída com sucesso. Total de vínculos: {Count}", vinculos.Count());

            return Ok(vinculos);
        }
        catch (OperationCanceledException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Requisição de listagem de vínculos cancelada");

            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar vínculos de questionário e bimestre");
            return StatusCode(500, new { mensagem = "Erro ao listar vínculos de questionário e bimestre" });
        }
    }

    [HttpGet("{questionarioId}")]
    [ProducesResponseType(typeof(IEnumerable<QuestionarioBimestreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorQuestionario(int questionarioId, CancellationToken cancellationToken)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Obtendo bimestres do questionário {QuestionarioId}", questionarioId);

            var vinculos = await _obterPorQuestionarioUseCase.ExecutarAsync(questionarioId, cancellationToken);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Consulta concluída. Total de bimestres vinculados: {Count}", vinculos.Count());

            return Ok(vinculos);
        }
        catch (OperationCanceledException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Requisição cancelada ao obter bimestres do questionário {QuestionarioId}", questionarioId);

            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Erro ao obter bimestres do questionário {QuestionarioId}", questionarioId);

            return StatusCode(500, new { mensagem = "Erro ao obter bimestres do questionário" });
        }
    }

    [HttpPost("vincular-multiplos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VincularMultiplos([FromBody] VincularBimestresDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Vinculando bimestres ao questionário {QuestionarioId}. Bimestres: [{BimestreIds}]",
                    dto.QuestionarioId, string.Join(", ", dto.BimestreIds));

            var sucesso = await _vincularBimestresUseCase.ExecutarAsync(dto, cancellationToken);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Bimestres vinculados com sucesso ao questionário {QuestionarioId}", dto.QuestionarioId);

            return Ok(new { sucesso, mensagem = "Bimestres vinculados com sucesso" });
        }
        catch (OperationCanceledException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Requisição cancelada ao vincular bimestres ao questionário {QuestionarioId}", dto.QuestionarioId);

            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (FluentValidation.ValidationException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Erro de validação ao vincular bimestres ao questionário {QuestionarioId}", dto.QuestionarioId);

            var erros = ex.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage });
            return BadRequest(new { mensagem = "Erro de validação", erros });
        }
        catch (RegraNegocioException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Regra de negócio violada ao vincular bimestres. QuestionarioId: {QuestionarioId}, StatusCode: {StatusCode}",
                    dto.QuestionarioId, ex.StatusCode);

            return StatusCode(ex.StatusCode, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Erro inesperado ao vincular bimestres ao questionário {QuestionarioId}", dto.QuestionarioId);

            return StatusCode(500, new { mensagem = "Erro ao vincular bimestres" });
        }
    }

    [HttpPut("{questionarioId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AtualizarVinculos(int questionarioId, [FromBody] AtualizarVinculosBimestresDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (dto.QuestionarioId.HasValue && dto.QuestionarioId.Value != questionarioId)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("ID do questionário na rota ({RouteId}) não corresponde ao do body ({BodyId})",
                        questionarioId, dto.QuestionarioId.Value);

                return BadRequest(new { mensagem = "O ID do questionário na rota não corresponde ao informado no corpo da requisição" });
            }

            dto.QuestionarioId = questionarioId;

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Atualizando vínculos do questionário {QuestionarioId}. Novos bimestres: [{BimestreIds}]",
                    questionarioId, dto.BimestreIds != null ? string.Join(", ", dto.BimestreIds) : "nenhum");

            var sucesso = await _vincularBimestresUseCase.ExecutarAtualizacaoAsync(dto, cancellationToken);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Vínculos atualizados com sucesso para o questionário {QuestionarioId}", questionarioId);

            return Ok(new { sucesso, mensagem = "Vínculos atualizados com sucesso" });
        }
        catch (OperationCanceledException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Requisição cancelada ao atualizar vínculos do questionário {QuestionarioId}", questionarioId);

            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (FluentValidation.ValidationException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Erro de validação ao atualizar vínculos do questionário {QuestionarioId}", questionarioId);

            var erros = ex.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage });
            return BadRequest(new { mensagem = "Erro de validação", erros });
        }
        catch (RegraNegocioException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Regra de negócio violada ao atualizar vínculos. QuestionarioId: {QuestionarioId}, StatusCode: {StatusCode}",
                    questionarioId, ex.StatusCode);

            return StatusCode(ex.StatusCode, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Erro inesperado ao atualizar vínculos do questionário {QuestionarioId}", questionarioId);

            return StatusCode(500, new { mensagem = "Erro ao atualizar vínculos" });
        }
    }

    [HttpDelete("{questionarioId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExcluirVinculos(int questionarioId, CancellationToken cancellationToken)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Excluindo vínculos do questionário {QuestionarioId}", questionarioId);

            var resultado = await _excluirPorQuestionarioUseCase.ExecutarAsync(questionarioId, cancellationToken);

            if (!resultado)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Nenhum vínculo encontrado para o questionário {QuestionarioId}", questionarioId);

                return NotFound(new { mensagem = $"Nenhum vínculo encontrado para o questionário {questionarioId}" });
            }

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Vínculos excluídos com sucesso para o questionário {QuestionarioId}", questionarioId);

            return NoContent();
        }
        catch (OperationCanceledException ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Requisição cancelada ao excluir vínculos do questionário {QuestionarioId}", questionarioId);

            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Erro ao excluir vínculos do questionário {QuestionarioId}", questionarioId);

            return StatusCode(500, new { mensagem = "Erro ao excluir vínculos" });
        }
    }
}