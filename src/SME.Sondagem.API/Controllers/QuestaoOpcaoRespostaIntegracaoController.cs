using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ChaveIntegracaoApiAttribute]
public class QuestaoOpcaoRespostaIntegracaoController : ControllerBase
{
    private readonly IObterQuestaoOpcaoRespostaUseCase obterQuestaoOpcaoRespostasUseCase;
    private readonly IObterQuestaoOpcaoRespostaPorIdUseCase obterQuestaoOpcaoRespostaPorIdUseCase;
    private readonly ICriarQuestaoOpcaoRespostaUseCase criarQuestaoOpcaoRespostaUseCase;
    private readonly IAtualizarQuestaoOpcaoRespostaUseCase atualizarQuestaoOpcaoRespostaUseCase;
    private readonly IExcluirQuestaoOpcaoRespostaUseCase excluirQuestaoOpcaoRespostaUseCase;

    public QuestaoOpcaoRespostaIntegracaoController(
        IObterQuestaoOpcaoRespostaUseCase obterQuestaoOpcaoRespostasUseCase,
        IObterQuestaoOpcaoRespostaPorIdUseCase obterQuestaoOpcaoRespostaPorIdUseCase,
        ICriarQuestaoOpcaoRespostaUseCase criarQuestaoOpcaoRespostaUseCase,
        IAtualizarQuestaoOpcaoRespostaUseCase atualizarQuestaoOpcaoRespostaUseCase,
        IExcluirQuestaoOpcaoRespostaUseCase excluirQuestaoOpcaoRespostaUseCase)
    {
        this.obterQuestaoOpcaoRespostasUseCase = obterQuestaoOpcaoRespostasUseCase;
        this.obterQuestaoOpcaoRespostaPorIdUseCase = obterQuestaoOpcaoRespostaPorIdUseCase;
        this.criarQuestaoOpcaoRespostaUseCase = criarQuestaoOpcaoRespostaUseCase;
        this.atualizarQuestaoOpcaoRespostaUseCase = atualizarQuestaoOpcaoRespostaUseCase;
        this.excluirQuestaoOpcaoRespostaUseCase = excluirQuestaoOpcaoRespostaUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestaoOpcaoRespostaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var resultado = await obterQuestaoOpcaoRespostasUseCase.ExecutarAsync(cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuestaoOpcaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var resultado = await obterQuestaoOpcaoRespostaPorIdUseCase.ExecutarAsync(id, cancellationToken);

        if (resultado == null)
            throw new RegraNegocioException(
                string.Format(MensagemNegocioComuns.QUESTAO_OPCAO_RESPOSTA_NAO_ENCONTRADA, id),
                StatusCodes.Status404NotFound);

        return Ok(resultado);
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestaoOpcaoRespostaDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] QuestaoOpcaoRespostaDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await criarQuestaoOpcaoRespostaUseCase.ExecutarAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = resultado },
            resultado);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(QuestaoOpcaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] QuestaoOpcaoRespostaDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await atualizarQuestaoOpcaoRespostaUseCase.ExecutarAsync(id, dto, cancellationToken);

        if (resultado == null)
            throw new RegraNegocioException(
                string.Format(MensagemNegocioComuns.QUESTAO_OPCAO_RESPOSTA_NAO_ENCONTRADA, id),
                StatusCodes.Status404NotFound);

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var sucesso = await excluirQuestaoOpcaoRespostaUseCase.ExecutarAsync(id, cancellationToken);

        if (!sucesso)
            throw new RegraNegocioException(
                string.Format(MensagemNegocioComuns.QUESTAO_OPCAO_RESPOSTA_NAO_ENCONTRADA, id),
                StatusCodes.Status404NotFound);

        return NoContent();
    }
}
