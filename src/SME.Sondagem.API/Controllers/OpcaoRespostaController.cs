using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class OpcaoRespostaController : ControllerBase
{
    private readonly ICriarOpcaoRespostaUseCase _criarOpcaoRespostaUseCase;
    private readonly IAtualizarOpcaoRespostaUseCase _atualizarOpcaoRespostaUseCase;
    private readonly IExcluirOpcaoRespostaUseCase _excluirOpcaoRespostaUseCase;
    private readonly IObterOpcaoRespostaPorIdUseCase _obterOpcaoRespostaPorIdUseCase;
    private readonly IObterOpcaoRespostaUseCase _obterOpcaoRespostaUseCase;

    public OpcaoRespostaController(
        ICriarOpcaoRespostaUseCase criarOpcaoRespostaUseCase,
        IAtualizarOpcaoRespostaUseCase atualizarOpcaoRespostaUseCase,
        IExcluirOpcaoRespostaUseCase excluirOpcaoRespostaUseCase,
        IObterOpcaoRespostaPorIdUseCase obterOpcaoRespostaPorIdUseCase,
        IObterOpcaoRespostaUseCase obterOpcaoRespostaUseCase)
    {
        _criarOpcaoRespostaUseCase = criarOpcaoRespostaUseCase;
        _atualizarOpcaoRespostaUseCase = atualizarOpcaoRespostaUseCase;
        _excluirOpcaoRespostaUseCase = excluirOpcaoRespostaUseCase;
        _obterOpcaoRespostaPorIdUseCase = obterOpcaoRespostaPorIdUseCase;
        _obterOpcaoRespostaUseCase = obterOpcaoRespostaUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OpcaoRespostaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var opcaoRespostas = await _obterOpcaoRespostaUseCase.ExecutarAsync(cancellationToken);
            return Ok(opcaoRespostas);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao listar opção resposta" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OpcaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(long id, CancellationToken cancellationToken)
    {
        try
        {
            var opcaoResposta = await _obterOpcaoRespostaPorIdUseCase.ExecutarAsync(id, cancellationToken);
            if (opcaoResposta == null)
                return NotFound(new { mensagem = $"Opção resposta com ID {id} não encontrado" });

            return Ok(opcaoResposta);
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
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] OpcaoRespostaDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _criarOpcaoRespostaUseCase.ExecutarAsync(dto, cancellationToken);
            return CreatedAtAction(
                nameof(ObterPorId),
                new { id },
                new { id }
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
    public async Task<IActionResult> Atualizar(long id, [FromBody] OpcaoRespostaDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var opcaoResposta = await _atualizarOpcaoRespostaUseCase.ExecutarAsync(id, dto, cancellationToken);
            if (opcaoResposta == null)
                return NotFound(new { mensagem = $"Opção resposta com ID {id} não encontrado" });

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
    public async Task<IActionResult> Excluir(long id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _excluirOpcaoRespostaUseCase.ExecutarAsync(id, cancellationToken);
            if (!resultado)
                return NotFound(new { mensagem = $"Opção resposta com ID {id} não encontrado" });

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
