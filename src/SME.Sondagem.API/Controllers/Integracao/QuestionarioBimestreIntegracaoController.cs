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

    public QuestionarioBimestreIntegracaoController(
        IVincularBimestresUseCase vincularBimestresUseCase,
        IObterQuestionariosBimestresUseCase obterTodosUseCase,
        IObterBimestresPorQuestionarioUseCase obterPorQuestionarioUseCase,
        IExcluirVinculosPorQuestionarioUseCase excluirPorQuestionarioUseCase)
    {
        _vincularBimestresUseCase = vincularBimestresUseCase;
        _obterTodosUseCase = obterTodosUseCase;
        _obterPorQuestionarioUseCase = obterPorQuestionarioUseCase;
        _excluirPorQuestionarioUseCase = excluirPorQuestionarioUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestionarioBimestreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var vinculos = await _obterTodosUseCase.ExecutarAsync(cancellationToken);
            return Ok(vinculos);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
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
            var vinculos = await _obterPorQuestionarioUseCase.ExecutarAsync(questionarioId, cancellationToken);
            return Ok(vinculos);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
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
            var sucesso = await _vincularBimestresUseCase.ExecutarAsync(dto, cancellationToken);
            return Ok(new { sucesso, mensagem = "Bimestres vinculados com sucesso" });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
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
                return BadRequest(new { mensagem = "O ID do questionário na rota não corresponde ao informado no corpo da requisição" });
            }

            dto.QuestionarioId = questionarioId;

            var sucesso = await _vincularBimestresUseCase.ExecutarAtualizacaoAsync(dto, cancellationToken);

            return Ok(new { sucesso, mensagem = "Vínculos atualizados com sucesso" });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
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
            var resultado = await _excluirPorQuestionarioUseCase.ExecutarAsync(questionarioId, cancellationToken);

            if (!resultado)
            {
                return NotFound(new { mensagem = $"Nenhum vínculo encontrado para o questionário {questionarioId}" });
            }

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao excluir vínculos" });
        }
    }
}