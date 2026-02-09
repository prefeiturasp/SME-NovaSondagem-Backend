using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Exceptions;

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
    private readonly IObterProficienciasPorComponenteCurricularUseCase obterProficienciasPorComponenteCurricularUseCase;

    public ProficienciaController(
        IObterProficienciasUseCase obterProficienciasUseCase,
        IObterProficienciaPorIdUseCase obterProficienciaPorIdUseCase,
        ICriarProficienciaUseCase criarProficienciaUseCase,
        IAtualizarProficienciaUseCase atualizarProficienciaUseCase,
        IExcluirProficienciaUseCase excluirProficienciaUseCase,
        IObterProficienciasPorComponenteCurricularUseCase obterProficienciasPorComponenteCurricularUseCase)
    {
        this.obterProficienciasUseCase = obterProficienciasUseCase;
        this.obterProficienciaPorIdUseCase = obterProficienciaPorIdUseCase;
        this.criarProficienciaUseCase = criarProficienciaUseCase;
        this.atualizarProficienciaUseCase = atualizarProficienciaUseCase;
        this.excluirProficienciaUseCase = excluirProficienciaUseCase;
        this.obterProficienciasPorComponenteCurricularUseCase = obterProficienciasPorComponenteCurricularUseCase;
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
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao listar proficiências" });
        }
    }

    [HttpGet("componente-curricular/{componenteCurricularId:long}/modalidade/{modalidadeId:long}")]
    [ProducesResponseType(typeof(IEnumerable<ProficienciaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterProeficienciaPorComponenteCurricular(long componenteCurricularId,long modalidadeId,
        CancellationToken cancellationToken)
    {
        try
        {
            var consulta =
                await obterProficienciasPorComponenteCurricularUseCase.ExecutarAsync(componenteCurricularId,modalidadeId,
                    cancellationToken);
            return Ok(consulta);
        }
        catch (NegocioException e)
        {
            return StatusCode(e.StatusCode, new { mensagem = e.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao obter proficiência" });
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
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.PROEFICIENCIA_NAO_ENCONTRADA, id) });

            return Ok(resultado);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao obter proficiência" });
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
            return StatusCode(500, new { mensagem = "Erro ao criar proficiência" });
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
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.PROEFICIENCIA_NAO_ENCONTRADA, id) });
                
            return Ok(proficiencia);
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
            return StatusCode(500, new { mensagem = "Erro ao atualizar proficiência" });
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
                return NotFound(new { mensagem = string.Format(MensagemNegocioComuns.PROEFICIENCIA_NAO_ENCONTRADA, id) });

            return NoContent();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensagem = "Erro ao excluir proficiência" });
        }
    }
}
