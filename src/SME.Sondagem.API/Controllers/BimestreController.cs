using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class BimestreController : ControllerBase
{
    private readonly ICriarBimestreUseCase _criarBimestreUseCase;
    private readonly IAtualizarBimestreUseCase _atualizarBimestreUseCase;
    private readonly IExcluirBimestreUseCase _excluirBimestreUseCase;
    private readonly IObterBimestrePorIdUseCase _obterBimestrePorIdUseCase;
    private readonly IObterBimestresUseCase _obterBimestreUseCase;
    public BimestreController(
        ICriarBimestreUseCase criarBimestreUseCase,
        IAtualizarBimestreUseCase atualizarBimestreUseCase,
        IExcluirBimestreUseCase excluirBimestreUseCase,
        IObterBimestrePorIdUseCase obterBimestrePorIdUseCase,
        IObterBimestresUseCase obterBimestreUseCase)
    {
        _criarBimestreUseCase = criarBimestreUseCase;
        _atualizarBimestreUseCase = atualizarBimestreUseCase;
        _excluirBimestreUseCase = excluirBimestreUseCase;
        _obterBimestrePorIdUseCase = obterBimestrePorIdUseCase;
        _obterBimestreUseCase = obterBimestreUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BimestreDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var bimestres = await _obterBimestreUseCase.ExecutarAsync(cancellationToken);
            return Ok(bimestres);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao listar bimestres" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BimestreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(long id, CancellationToken cancellationToken)
    {
        try
        {
            var bimestre = await _obterBimestrePorIdUseCase.ExecutarAsync(id, cancellationToken);

            if (bimestre == null)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.BIMESTRE_NAO_ENCONTRADO, id) });

            return Ok(bimestre);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao obter bimestre" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] BimestreDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _criarBimestreUseCase.ExecutarAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id },
                new { id });
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
            return StatusCode(500, new { mensagem = "Erro ao criar bimestre" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(long id, [FromBody] BimestreDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var bimestre = await _atualizarBimestreUseCase.ExecutarAsync(id, dto, cancellationToken);

            if (bimestre == null)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.BIMESTRE_NAO_ENCONTRADO, id)});

            return Ok(bimestre);
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
            return StatusCode(500, new { mensagem = "Erro ao atualizar bimestre" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(long id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _excluirBimestreUseCase.ExecutarAsync(id, cancellationToken);

            if (!resultado)
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.BIMESTRE_NAO_ENCONTRADO, id) });

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao excluir bimestre" });
        }
    }
}
