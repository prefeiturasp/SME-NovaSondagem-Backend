using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers.Integracao;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class QuestionarioIntegracaoController : ControllerBase
{
    private readonly IObterQuestionariosUseCase obterQuestionariosUseCase;
    private readonly IObterQuestionarioPorIdUseCase obterQuestionarioPorIdUseCase;
    private readonly ICriarQuestionarioUseCase criarQuestionarioUseCase;
    private readonly IAtualizarQuestionarioUseCase atualizarQuestionarioUseCase;
    private readonly IExcluirQuestionarioUseCase excluirQuestionarioUseCase;

    public QuestionarioIntegracaoController(
        IObterQuestionariosUseCase obterQuestionariosUseCase,
        IObterQuestionarioPorIdUseCase obterQuestionarioPorIdUseCase,
        ICriarQuestionarioUseCase criarQuestionarioUseCase,
        IAtualizarQuestionarioUseCase atualizarQuestionarioUseCase,
        IExcluirQuestionarioUseCase excluirQuestionarioUseCase)
    {
        this.obterQuestionariosUseCase = obterQuestionariosUseCase;
        this.obterQuestionarioPorIdUseCase = obterQuestionarioPorIdUseCase;
        this.criarQuestionarioUseCase = criarQuestionarioUseCase;
        this.atualizarQuestionarioUseCase = atualizarQuestionarioUseCase;
        this.excluirQuestionarioUseCase = excluirQuestionarioUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestionarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var resultado = await obterQuestionariosUseCase.ExecutarAsync(cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuestionarioDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var resultado = await obterQuestionarioPorIdUseCase.ExecutarAsync(id, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, id));

        return Ok(resultado);
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestionarioDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] QuestionarioDto questionarioDto,
        CancellationToken cancellationToken)
    {
        var id = await criarQuestionarioUseCase.ExecutarAsync(questionarioDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(QuestionarioDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] QuestionarioDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await atualizarQuestionarioUseCase.ExecutarAsync(id, dto, cancellationToken);

        if (resultado == null)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, id));

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var sucesso = await excluirQuestionarioUseCase.ExecutarAsync(id, cancellationToken);

        if (!sucesso)
            throw new ErroNaoEncontradoException(string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, id));

        return NoContent();
    }
}
