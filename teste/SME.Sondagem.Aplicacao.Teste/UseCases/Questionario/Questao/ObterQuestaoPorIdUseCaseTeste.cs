using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questao;

public class ObterQuestaoPorIdUseCaseTeste
{
    private readonly Mock<IRepositorioQuestao> _repositorioQuestaoMock;
    private readonly ObterQuestaoPorIdUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterQuestaoPorIdUseCaseTeste()
    {
        _repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        _useCase = new ObterQuestaoPorIdUseCase(_repositorioQuestaoMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    // Método auxiliar para criar instâncias de Questao nos testes
    private static SME.Sondagem.Dominio.Entidades.Questionario.Questao CriarQuestao(
        int questionarioId = 1,
        int ordem = 1,
        string nome = "Questao Teste",
        string observacao = "Observacao teste",
        bool obrigatorio = true,
        TipoQuestao tipo = TipoQuestao.Texto,
        string opcionais = "{}",
        bool somenteLeitura = false,
        int dimensao = 100,
        int? grupoQuestoesId = null,
        int? tamanho = null,
        string? mascara = null,
        string? placeHolder = null,
        string? nomeComponente = null,
        int? id = null,
        DateTime? criadoEm = null,
        string? criadoPor = null,
        string? criadoRF = null,
        DateTime? alteradoEm = null,
        string? alteradoPor = null,
        string? alteradoRF = null)
    {
        var questao = new SME.Sondagem.Dominio.Entidades.Questionario.Questao(
            questionarioId: questionarioId,
            ordem: ordem,
            nome: nome,
            observacao: observacao,
            obrigatorio: obrigatorio,
            tipo: tipo,
            opcionais: opcionais,
            somenteLeitura: somenteLeitura,
            dimensao: dimensao,
            grupoQuestoesId: grupoQuestoesId,
            tamanho: tamanho,
            mascara: mascara,
            placeHolder: placeHolder,
            nomeComponente: nomeComponente
        );

        // Usa reflexão para definir propriedades da classe base
        if (id.HasValue)
        {
            var idProp = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("Id");
            idProp?.SetValue(questao, id.Value);
        }

        if (criadoEm.HasValue)
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("CriadoEm");
            prop?.SetValue(questao, criadoEm.Value);
        }

        if (!string.IsNullOrEmpty(criadoPor))
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("CriadoPor");
            prop?.SetValue(questao, criadoPor);
        }

        if (!string.IsNullOrEmpty(criadoRF))
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("CriadoRF");
            prop?.SetValue(questao, criadoRF);
        }

        if (alteradoEm.HasValue)
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("AlteradoEm");
            prop?.SetValue(questao, alteradoEm);
        }

        if (!string.IsNullOrEmpty(alteradoPor))
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("AlteradoPor");
            prop?.SetValue(questao, alteradoPor);
        }

        if (!string.IsNullOrEmpty(alteradoRF))
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("AlteradoRF");
            prop?.SetValue(questao, alteradoRF);
        }

        return questao;
    }

    [Fact]
    public async Task ExecutarAsync_QuestaoExiste_DeveRetornarQuestaoDto()
    {
        const int id = 1;
        var agora = DateTime.Now;
        var agoraAlterado = agora.AddHours(1);

        var questao = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: 2,
            ordem: 1,
            nome: "Questao Teste",
            observacao: "Observacao teste",
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: "{}",
            somenteLeitura: false,
            dimensao: 100,
            tamanho: 50,
            mascara: null,
            placeHolder: "Digite aqui",
            nomeComponente: "input-text",
            id: (int)id,
            criadoEm: agora,
            criadoPor: "Usuario1",
            criadoRF: "RF001",
            alteradoEm: agoraAlterado,
            alteradoPor: "Usuario2",
            alteradoRF: "RF002"
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: _cancellationToken))
            .ReturnsAsync(questao);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Questao Teste", resultado.Nome);
        Assert.Equal(2, resultado.GrupoQuestoesId);
        Assert.Equal(1, resultado.QuestionarioId);
        Assert.Equal(1, resultado.Ordem);
        Assert.Equal("Observacao teste", resultado.Observacao);
        Assert.True(resultado.Obrigatorio);
        Assert.Equal(TipoQuestao.Texto, resultado.Tipo);
        Assert.Equal("Usuario1", resultado.CriadoPor);
        Assert.Equal("RF001", resultado.CriadoRF);
        Assert.Equal(questao.CriadoEm, resultado.CriadoEm);
        Assert.Equal(questao.AlteradoEm, resultado.AlteradoEm);
        Assert.Equal("Usuario2", resultado.AlteradoPor);
        Assert.Equal("RF002", resultado.AlteradoRF);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuestaoNaoExiste_DeveRetornarNull()
    {
        const long id = 999;

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.Questao?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.Null(resultado);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComParametrosCorretos()
    {
        const long id = 42;
        var cancellationTokenCustom = new CancellationToken(false);

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: cancellationTokenCustom))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.Questao?)null);

        await _useCase.ExecutarAsync(id, cancellationTokenCustom);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: cancellationTokenCancelado), Times.Once);
    }
}