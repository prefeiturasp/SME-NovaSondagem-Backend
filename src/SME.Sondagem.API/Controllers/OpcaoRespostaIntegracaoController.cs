using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class OpcaoRespostaIntegracaoController : ControllerBase
{
    private readonly IObterOpcaoRespostaUseCase obterOpcaoRespostasUseCase;
    private readonly IObterOpcaoRespostaPorIdUseCase obterOpcaoRespostaPorIdUseCase;
    private readonly ICriarOpcaoRespostaUseCase criarOpcaoRespostaUseCase;
    private readonly IAtualizarOpcaoRespostaUseCase atualizarOpcaoRespostaUseCase;
    private readonly IExcluirOpcaoRespostaUseCase excluirOpcaoRespostaUseCase;

    public OpcaoRespostaIntegracaoController(
        IObterOpcaoRespostaUseCase obterOpcaoRespostasUseCase,
        IObterOpcaoRespostaPorIdUseCase obterOpcaoRespostaPorIdUseCase,
        ICriarOpcaoRespostaUseCase criarOpcaoRespostaUseCase,
        IAtualizarOpcaoRespostaUseCase atualizarOpcaoRespostaUseCase,
        IExcluirOpcaoRespostaUseCase excluirOpcaoRespostaUseCase)
    {
        this.obterOpcaoRespostasUseCase = obterOpcaoRespostasUseCase;
        this.obterOpcaoRespostaPorIdUseCase = obterOpcaoRespostaPorIdUseCase;
        this.criarOpcaoRespostaUseCase = criarOpcaoRespostaUseCase;
        this.atualizarOpcaoRespostaUseCase = atualizarOpcaoRespostaUseCase;
        this.excluirOpcaoRespostaUseCase = excluirOpcaoRespostaUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OpcaoRespostaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await obterOpcaoRespostasUseCase.ExecutarAsync(cancellationToken);
            return Ok(resultado);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao listar opções respostas" });
        }
    }        

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OpcaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await obterOpcaoRespostaPorIdUseCase.ExecutarAsync(id, cancellationToken);

            if (resultado == null)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.OPCAO_RESPOSTA_NAO_ENCONTRADA, id) });

            return Ok(resultado);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao obter opção resposta" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(OpcaoRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] OpcaoRespostaDto opcaoRespostaDto, CancellationToken cancellationToken)
    {
        try
        {
            var opcaoResposta = await criarOpcaoRespostaUseCase.ExecutarAsync(opcaoRespostaDto, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = opcaoResposta },
                opcaoResposta
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
            return StatusCode(500, new { mensagem = "Erro ao criar opção resposta" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OpcaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] OpcaoRespostaDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var opcaoResposta = await atualizarOpcaoRespostaUseCase.ExecutarAsync(id, dto, cancellationToken);

            if (opcaoResposta == null)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.OPCAO_RESPOSTA_NAO_ENCONTRADA, id) });

            return Ok(opcaoResposta);
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
            return StatusCode(500, new { mensagem = "Erro ao atualizar opção resposta" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await excluirOpcaoRespostaUseCase.ExecutarAsync(id, cancellationToken);
            if (!resultado)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.OPCAO_RESPOSTA_NAO_ENCONTRADA, id) });

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao excluir opção resposta" });
        }
    }
}
