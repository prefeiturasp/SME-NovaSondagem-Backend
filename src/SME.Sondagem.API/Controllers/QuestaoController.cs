using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.Questao;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class QuestaoController : ControllerBase
{
    private readonly IObterQuestoesUseCase _obterQuestoesUseCase;
    private readonly IObterQuestaoPorIdUseCase _obterQuestaoPorIdUseCase;
    private readonly ICriarQuestaoUseCase _criarQuestaoUseCase;
    private readonly IAtualizarQuestaoUseCase _atualizarQuestaoUseCase;
    private readonly IExcluirQuestaoUseCase _excluirQuestaoUseCase;
    private readonly ILogger<QuestaoController> _logger;

    public QuestaoController(
        IObterQuestoesUseCase obterQuestoesUseCase,
        IObterQuestaoPorIdUseCase obterQuestaoPorIdUseCase,
        ICriarQuestaoUseCase criarQuestaoUseCase,
        IAtualizarQuestaoUseCase atualizarQuestaoUseCase,
        IExcluirQuestaoUseCase excluirQuestaoUseCase,
        ILogger<QuestaoController> logger)
    {
        _obterQuestoesUseCase = obterQuestoesUseCase;
        _obterQuestaoPorIdUseCase = obterQuestaoPorIdUseCase;
        _criarQuestaoUseCase = criarQuestaoUseCase;
        _atualizarQuestaoUseCase = atualizarQuestaoUseCase;
        _excluirQuestaoUseCase = excluirQuestaoUseCase;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestaoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var questoes = await _obterQuestoesUseCase.ExecutarAsync(cancellationToken);
            return Ok(questoes);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Requisição de listagem foi cancelada");
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar questões");
            return StatusCode(500, new { mensagem = "Erro ao listar questões" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuestaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
    {
        try
        {
            var questao = await _obterQuestaoPorIdUseCase.ExecutarAsync(id, cancellationToken);
            if (questao == null)
                return NotFound(new { mensagem = $"Questão com ID {id} não encontrada" });

            return Ok(questao);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Requisição de obtenção foi cancelada para ID {Id}", id);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter questão {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao obter questão" });
        }
    }   

    [HttpPost]
    [ProducesResponseType(typeof(QuestaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] QuestaoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var questaoId = await _criarQuestaoUseCase.ExecutarAsync(dto, cancellationToken);
            var questao = await _obterQuestaoPorIdUseCase.ExecutarAsync(questaoId, cancellationToken);
            
            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = questaoId },
                questao
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
            _logger.LogError(ex, "Erro ao criar questão");
            return StatusCode(500, new { mensagem = "Erro ao criar questão" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(QuestaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] QuestaoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var questao = await _atualizarQuestaoUseCase.ExecutarAsync(id, dto, cancellationToken);
            if (questao == null)
                return NotFound(new { mensagem = $"Questão com ID {id} não encontrada" });

            return Ok(questao);
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
            _logger.LogError(ex, "Erro ao atualizar questão {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao atualizar questão" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _excluirQuestaoUseCase.ExecutarAsync(id, cancellationToken);
            if (!resultado)
                return NotFound(new { mensagem = $"Questão com ID {id} não encontrada" });

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Requisição de exclusão foi cancelada para ID {Id}", id);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir questão {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao excluir questão" });
        }
    }
}
