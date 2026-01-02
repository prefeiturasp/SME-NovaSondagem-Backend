using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AutenticacaoSettingsApi.BearerTokenSondagem)]
public class ProficienciaController : ControllerBase
{
    private readonly IObterProficienciasUseCase obterProficienciasUseCase;
    private readonly IObterProficienciaPorIdUseCase obterProficienciaPorIdUseCase;
    private readonly ICriarProficienciaUseCase criarProficienciaUseCase;
    private readonly IAtualizarProficienciaUseCase atualizarProficienciaUseCase;
    private readonly IExcluirProficienciaUseCase excluirProficienciaUseCase;

    private readonly ILogger<ComponenteCurricularController> _logger;

    public ProficienciaController(
        IObterProficienciasUseCase obterProficienciasUseCase,
        IObterProficienciaPorIdUseCase obterProficienciaPorIdUseCase,
        ICriarProficienciaUseCase criarProficienciaUseCase,
        IAtualizarProficienciaUseCase atualizarProficienciaUseCase,
        IExcluirProficienciaUseCase excluirProficienciaUseCase,
        ILogger<ComponenteCurricularController> logger)
    {
        this.obterProficienciasUseCase = obterProficienciasUseCase;
        this.obterProficienciaPorIdUseCase = obterProficienciaPorIdUseCase;
        this.criarProficienciaUseCase = criarProficienciaUseCase;
        this.atualizarProficienciaUseCase = atualizarProficienciaUseCase;
        this.excluirProficienciaUseCase = excluirProficienciaUseCase;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProficienciaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await obterProficienciasUseCase.ExecutarAsync(cancellationToken);
            return Ok(resultado);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Requisição de listagem foi cancelada");
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar proficiãncias");
            return StatusCode(500, new { mensagem = "Erro ao listar proficiãncias" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProficienciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await obterProficienciaPorIdUseCase.ExecutarAsync(id, cancellationToken);

            if (resultado == null)
                return NotFound(new { mensagem = $"Proficiãncia com ID {id} não encontrada" });

            return Ok(resultado);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Requisição de obtenção foi cancelada para ID {Id}", id);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter proficiãncia {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao obter proficiãncia" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProficienciaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] ProficienciaDto proficienciaDto, CancellationToken cancellationToken)
    {
        try
        {
            var proficiencia = await criarProficienciaUseCase.ExecutarAsync(proficienciaDto, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = proficiencia },
                proficiencia
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
            _logger.LogError(ex, "Erro ao criar proficiãncia");
            return StatusCode(500, new { mensagem = "Erro ao criar proficiãncia" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProficienciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ProficienciaDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var proficiencia = await atualizarProficienciaUseCase.ExecutarAsync(id, dto, cancellationToken);
            
            if (proficiencia == null)
                return NotFound(new { mensagem = $"Proficiãncia com ID {id} não encontrada" });
                
            return Ok(proficiencia);
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
            _logger.LogError(ex, "Erro ao atualizar proficiãncia {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao atualizar proficiãncia" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await excluirProficienciaUseCase.ExecutarAsync(id, cancellationToken);
            if (!resultado)
                return NotFound(new { mensagem = $"Proficiãncia com ID {id} não encontrada" });

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Requisição de exclusão foi cancelada para ID {Id}", id);
            return StatusCode(499, new { mensagem = "Requisição cancelada pelo cliente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir proficiãncia {Id}", id);
            return StatusCode(500, new { mensagem = "Erro ao excluir proficiãncia" });
        }
    }
}
