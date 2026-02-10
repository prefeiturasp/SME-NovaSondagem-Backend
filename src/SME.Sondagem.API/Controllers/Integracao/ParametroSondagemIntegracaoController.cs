using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers.Integracao;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class ParametroSondagemIntegracaoController : ControllerBase
{
    private readonly IObterParametrosSondagemUseCase obterParametrosSondagensUseCase;
    private readonly IObterParametroSondagemPorIdUseCase obterParametroSondagemPorIdUseCase;
    private readonly ICriarParametroSondagemUseCase criarParametroSondagemUseCase;
    private readonly IAtualizarParametroSondagemUseCase atualizarParametroSondagemUseCase;
    private readonly IExcluirParametroSondagemUseCase excluirParametroSondagemUseCase;

    public ParametroSondagemIntegracaoController(
        IObterParametrosSondagemUseCase obterParametrosSondagensUseCase,
        IObterParametroSondagemPorIdUseCase obterParametroSondagemPorIdUseCase,
        ICriarParametroSondagemUseCase criarParametroSondagemUseCase,
        IAtualizarParametroSondagemUseCase atualizarParametroSondagemUseCase,
        IExcluirParametroSondagemUseCase excluirParametroSondagemUseCase)
    {
        this.obterParametrosSondagensUseCase = obterParametrosSondagensUseCase;
        this.obterParametroSondagemPorIdUseCase = obterParametroSondagemPorIdUseCase;
        this.criarParametroSondagemUseCase = criarParametroSondagemUseCase;
        this.atualizarParametroSondagemUseCase = atualizarParametroSondagemUseCase;
        this.excluirParametroSondagemUseCase = excluirParametroSondagemUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ParametroSondagemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var resultado = await obterParametrosSondagensUseCase.ExecutarAsync(cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ParametroSondagemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var resultado = await obterParametroSondagemPorIdUseCase.ExecutarAsync(id, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.PARAMETRO_SONDAGEM_NAO_ENCONTRADO, id));

        return Ok(resultado);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ParametroSondagemDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] ParametroSondagemDto questionarioDto,
        CancellationToken cancellationToken)
    {
        var id = await criarParametroSondagemUseCase.ExecutarAsync(questionarioDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ParametroSondagemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] ParametroSondagemDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await atualizarParametroSondagemUseCase.ExecutarAsync(id, dto, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.PARAMETRO_SONDAGEM_NAO_ENCONTRADO, id));

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var sucesso = await excluirParametroSondagemUseCase.ExecutarAsync(id, cancellationToken);

        if (!sucesso)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.PARAMETRO_SONDAGEM_NAO_ENCONTRADO, id));

        return NoContent();
    }
}
