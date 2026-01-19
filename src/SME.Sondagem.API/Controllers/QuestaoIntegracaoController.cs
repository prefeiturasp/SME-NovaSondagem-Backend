using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class QuestaoIntegracaoController : ControllerBase
{
    private readonly IObterQuestoesUseCase obterQuestoesUseCase;
    private readonly IObterQuestaoPorIdUseCase obterQuestaoPorIdUseCase;
    private readonly ICriarQuestaoUseCase criarQuestaoUseCase;
    private readonly IAtualizarQuestaoUseCase atualizarQuestaoUseCase;
    private readonly IExcluirQuestaoUseCase excluirQuestaoUseCase;

    public QuestaoIntegracaoController(
        IObterQuestoesUseCase obterQuestoesUseCase,
        IObterQuestaoPorIdUseCase obterQuestaoPorIdUseCase,
        ICriarQuestaoUseCase criarQuestaoUseCase,
        IAtualizarQuestaoUseCase atualizarQuestaoUseCase,
        IExcluirQuestaoUseCase excluirQuestaoUseCase)
    {
        this.obterQuestoesUseCase = obterQuestoesUseCase;
        this.obterQuestaoPorIdUseCase = obterQuestaoPorIdUseCase;
        this.criarQuestaoUseCase = criarQuestaoUseCase;
        this.atualizarQuestaoUseCase = atualizarQuestaoUseCase;
        this.excluirQuestaoUseCase = excluirQuestaoUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestaoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await obterQuestoesUseCase.ExecutarAsync(cancellationToken);
            return Ok(resultado);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao listar questões" });
        }
    }        

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuestaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await obterQuestaoPorIdUseCase.ExecutarAsync(id, cancellationToken);

            if (resultado == null)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.QUESTAO_NAO_ENCONTRADA, id) });

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
    [ProducesResponseType(typeof(QuestaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] QuestaoDto questaoDto, CancellationToken cancellationToken)
    {
        try
        {
            var questao = await criarQuestaoUseCase.ExecutarAsync(questaoDto, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = questao },
                questao
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
            var questao = await atualizarQuestaoUseCase.ExecutarAsync(id, dto, cancellationToken);

            if (questao == null)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.QUESTAO_NAO_ENCONTRADA, id) });

            return Ok(questao);
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
            var resultado = await excluirQuestaoUseCase.ExecutarAsync(id, cancellationToken);
            if (!resultado)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.QUESTAO_NAO_ENCONTRADA, id) });

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao excluir questão" });
        }
    }
}
