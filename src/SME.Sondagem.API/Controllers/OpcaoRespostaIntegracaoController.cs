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
        var resultado = await obterOpcaoRespostasUseCase.ExecutarAsync(cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OpcaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var resultado = await obterOpcaoRespostaPorIdUseCase.ExecutarAsync(id, cancellationToken);

        if (resultado == null)
            throw new RegraNegocioException(
                string.Format(MensagemNegocioComuns.OPCAO_RESPOSTA_NAO_ENCONTRADA, id),
                StatusCodes.Status404NotFound);

        return Ok(resultado);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OpcaoRespostaDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] OpcaoRespostaDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await criarOpcaoRespostaUseCase.ExecutarAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = resultado },
            resultado);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OpcaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] OpcaoRespostaDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await atualizarOpcaoRespostaUseCase.ExecutarAsync(id, dto, cancellationToken);

        if (resultado == null)
            throw new RegraNegocioException(
                string.Format(MensagemNegocioComuns.OPCAO_RESPOSTA_NAO_ENCONTRADA, id),
                StatusCodes.Status404NotFound);

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var sucesso = await excluirOpcaoRespostaUseCase.ExecutarAsync(id, cancellationToken);

        if (!sucesso)
            throw new RegraNegocioException(
                string.Format(MensagemNegocioComuns.OPCAO_RESPOSTA_NAO_ENCONTRADA, id),
                StatusCodes.Status404NotFound);

        return NoContent();
    }
}
