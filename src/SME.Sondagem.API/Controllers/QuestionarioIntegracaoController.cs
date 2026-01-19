using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApi]
public class QuestionarioIntegracaoController : ControllerBase
{
    private readonly IObterQuestionariosUseCase obterQuestionariosUseCase;
    private readonly IObterQuestionarioPorIdUseCase obterQuestionarioPorIdUseCase;
    private readonly ICriarQuestionarioUseCase criarQuestionarioUseCase;
    private readonly IAtualizarQuestionarioUseCase atualizarQuestionarioUseCase;
    private readonly IExcluirQuestionarioUseCase excluirQuestionarioUseCase;

    public QuestionarioIntegracaoController(
        IObterQuestionariosUseCase obterQuestionariosUseCase,
        IObterQuestionarioPorIdUseCase obterQuestionarioPorIdUseCase,
        ICriarQuestionarioUseCase criarQuestionarioUseCase,
        IAtualizarQuestionarioUseCase atualizarQuestionarioUseCase,
        IExcluirQuestionarioUseCase excluirQuestionarioUseCase)
    {
        this.obterQuestionariosUseCase = obterQuestionariosUseCase;
        this.obterQuestionarioPorIdUseCase = obterQuestionarioPorIdUseCase;
        this.criarQuestionarioUseCase = criarQuestionarioUseCase;
        this.atualizarQuestionarioUseCase = atualizarQuestionarioUseCase;
        this.excluirQuestionarioUseCase = excluirQuestionarioUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestionarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await obterQuestionariosUseCase.ExecutarAsync(cancellationToken);
            return Ok(resultado);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao listar questionários" });
        }
    }        

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuestionarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await obterQuestionarioPorIdUseCase.ExecutarAsync(id, cancellationToken);

            if (resultado == null)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, id) });

            return Ok(resultado);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao obter questionário" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestionarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] QuestionarioDto questionarioDto, CancellationToken cancellationToken)
    {
        try
        {
            var questionario = await criarQuestionarioUseCase.ExecutarAsync(questionarioDto, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = questionario },
                questionario
            );
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
            return StatusCode(500, new { mensagem = "Erro ao criar questionário" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(QuestionarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] QuestionarioDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var questionario = await atualizarQuestionarioUseCase.ExecutarAsync(id, dto, cancellationToken);

            if (questionario == null)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, id) });

            return Ok(questionario);
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
            return StatusCode(500, new { mensagem = "Erro ao atualizar questionário" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await excluirQuestionarioUseCase.ExecutarAsync(id, cancellationToken);
            if (!resultado)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, id) });

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao excluir questionário" });
        }
    }
}
