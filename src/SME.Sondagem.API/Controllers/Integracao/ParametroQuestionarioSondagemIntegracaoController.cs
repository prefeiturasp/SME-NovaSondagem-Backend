using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers.Integracao;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class ParametroSondagemQuestionarioIntegracaoController : ControllerBase
{
    private readonly IObterParametrosSondagemQuestionarioUseCase obterParametrosSondagensQuestionarioUseCase;
    private readonly IObterParametroSondagemQuestionarioPorIdUseCase obterParametroSondagemQuestionarioPorIdUseCase;
    private readonly ICriarParametroSondagemQuestionarioUseCase criarParametroSondagemQuestionarioUseCase;
    private readonly IAtualizarParametroSondagemQuestionarioUseCase atualizarParametroSondagemQuestionarioUseCase;
    private readonly IExcluirParametroSondagemQuestionarioUseCase excluirParametroSondagemQuestionarioUseCase;

    public ParametroSondagemQuestionarioIntegracaoController(
        IObterParametrosSondagemQuestionarioUseCase obterParametrosSondagensQuestionarioUseCase,
        IObterParametroSondagemQuestionarioPorIdUseCase obterParametroSondagemQuestionarioPorIdUseCase,
        ICriarParametroSondagemQuestionarioUseCase criarParametroSondagemQuestionarioUseCase,
        IAtualizarParametroSondagemQuestionarioUseCase atualizarParametroSondagemQuestionarioUseCase,
        IExcluirParametroSondagemQuestionarioUseCase excluirParametroSondagemQuestionarioUseCase)
    {
        this.obterParametrosSondagensQuestionarioUseCase = obterParametrosSondagensQuestionarioUseCase;
        this.obterParametroSondagemQuestionarioPorIdUseCase = obterParametroSondagemQuestionarioPorIdUseCase;
        this.criarParametroSondagemQuestionarioUseCase = criarParametroSondagemQuestionarioUseCase;
        this.atualizarParametroSondagemQuestionarioUseCase = atualizarParametroSondagemQuestionarioUseCase;
        this.excluirParametroSondagemQuestionarioUseCase = excluirParametroSondagemQuestionarioUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ParametroSondagemQuestionarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var resultado = await obterParametrosSondagensQuestionarioUseCase.ExecutarAsync(cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ParametroSondagemQuestionarioDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var resultado = await obterParametroSondagemQuestionarioPorIdUseCase.ExecutarAsync(id, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.PARAMETRO_SONDAGEM_NAO_ENCONTRADO, id));

        return Ok(resultado);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ParametroSondagemQuestionarioDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] ParametroSondagemQuestionarioDto questionarioDto,
        CancellationToken cancellationToken)
    {
        var id = await criarParametroSondagemQuestionarioUseCase.ExecutarAsync(questionarioDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ParametroSondagemQuestionarioDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] ParametroSondagemQuestionarioDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await atualizarParametroSondagemQuestionarioUseCase.ExecutarAsync(id, dto, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.PARAMETRO_SONDAGEM_NAO_ENCONTRADO, id));

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var sucesso = await excluirParametroSondagemQuestionarioUseCase.ExecutarAsync(id, cancellationToken);

        if (!sucesso)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.PARAMETRO_SONDAGEM_NAO_ENCONTRADO, id));

        return NoContent();
    }
}
