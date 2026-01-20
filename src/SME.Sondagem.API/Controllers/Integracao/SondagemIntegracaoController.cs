using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.API.Controllers.Integracao;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class SondagemIntegracaoController : ControllerBase
{
    private readonly IObterSondagensUseCase obterSondagemsUseCase;
    private readonly IObterSondagemPorIdUseCase obterSondagemPorIdUseCase;
    private readonly ICriarSondagemUseCase criarSondagemUseCase;
    private readonly IAtualizarSondagemUseCase atualizarSondagemUseCase;
    private readonly IExcluirSondagemUseCase excluirSondagemUseCase;

    public SondagemIntegracaoController(
        IObterSondagensUseCase obterSondagemsUseCase,
        IObterSondagemPorIdUseCase obterSondagemPorIdUseCase,
        ICriarSondagemUseCase criarSondagemUseCase,
        IAtualizarSondagemUseCase atualizarSondagemUseCase,
        IExcluirSondagemUseCase excluirSondagemUseCase)
    {
        this.obterSondagemsUseCase = obterSondagemsUseCase;
        this.obterSondagemPorIdUseCase = obterSondagemPorIdUseCase;
        this.criarSondagemUseCase = criarSondagemUseCase;
        this.atualizarSondagemUseCase = atualizarSondagemUseCase;
        this.excluirSondagemUseCase = excluirSondagemUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SondagemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var resultado = await obterSondagemsUseCase.ExecutarAsync(cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SondagemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var resultado = await obterSondagemPorIdUseCase.ExecutarAsync(id, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException($"Sondagem {id} não encontrada");

        return Ok(resultado);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SondagemDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] SondagemDto sondagemDto,
        CancellationToken cancellationToken)
    {
        var id = await criarSondagemUseCase.ExecutarAsync(sondagemDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SondagemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] SondagemDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await atualizarSondagemUseCase.ExecutarAsync(id, dto, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException($"Sondagem {id} não encontrada");

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var sucesso = await excluirSondagemUseCase.ExecutarAsync(id, cancellationToken);

        if (!sucesso)
            throw new ErroNaoEncontradoException($"Sondagem {id} não encontrada");

        return NoContent();
    }
}
