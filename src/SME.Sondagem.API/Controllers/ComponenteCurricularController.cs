using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public ComponenteCurricularController(
        IComponenteCurricularUseCase useCase,
        ILogger<ComponenteCurricularController> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ComponenteCurricularDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        try
        {
            var componentes = await _useCase.ListarAsync();
            return Ok(componentes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar componentes curriculares");
            return StatusCode(500, new { mensagem = "Erro ao listar componentes curriculares" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ComponenteCurricularDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var componente = await _useCase.ObterPorIdAsync(id);
            if (componente == null)
                return NotFound(new { mensagem = $"Componente curricular com ID {id} não encontrado" });

            return Ok(componente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter componente curricular {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao obter componente curricular" });
        }
    }

    [HttpGet("codigo-eol/{codigoEol}")]
    [ProducesResponseType(typeof(ComponenteCurricularDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorCodigoEol(int codigoEol)
    {
        try
        {
            var componente = await _useCase.ObterPorCodigoEolAsync(codigoEol);
            if (componente == null)
                return NotFound(new { mensagem = $"Componente curricular com código EOL {codigoEol} não encontrado" });

            return Ok(componente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter componente curricular por código EOL {CodigoEol}", codigoEol);
            return StatusCode(500, new { mensagem = "Erro ao obter componente curricular" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ComponenteCurricularDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] CriarComponenteCurricularDto dto)
    {
        try
        {
            var componente = await _useCase.CriarAsync(dto);
            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = componente.Id },
                componente
            );
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
            _logger.LogError(ex, "Erro ao criar componente curricular");
            return StatusCode(500, new { mensagem = "Erro ao criar componente curricular" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ComponenteCurricularDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarComponenteCurricularDto dto)
    {
        try
        {
            var componente = await _useCase.AtualizarAsync(id, dto);
            return Ok(componente);
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
            _logger.LogError(ex, "Erro ao atualizar componente curricular {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao atualizar componente curricular" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            var resultado = await _useCase.ExcluirAsync(id);
            if (!resultado)
                return NotFound(new { mensagem = $"Componente curricular com ID {id} não encontrado" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir componente curricular {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao excluir componente curricular" });
        }
    }
}