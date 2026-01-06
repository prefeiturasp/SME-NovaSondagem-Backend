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


    public ComponenteCurricularController(
        IComponenteCurricularUseCase useCase)
    {
        _useCase = useCase;
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
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception)
        {
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
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception)
        {
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
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception)
        {
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
        catch (Exception)
        {
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
        catch (Exception)
        {
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
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao excluir componente curricular" });
        }
    }
}
