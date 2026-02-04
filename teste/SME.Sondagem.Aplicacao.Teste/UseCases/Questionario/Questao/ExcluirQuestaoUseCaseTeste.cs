using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questao;

public class ExcluirQuestaoUseCaseTeste
{
    private readonly Mock<IRepositorioQuestao> _repositorioQuestaoMock;
    private readonly ExcluirQuestaoUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ExcluirQuestaoUseCaseTeste()
    {
        _repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        _useCase = new ExcluirQuestaoUseCase(_repositorioQuestaoMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    // M�todo auxiliar para criar inst�ncias de Questao nos testes
    private static SME.Sondagem.Dominio.Entidades.Questionario.Questao CriarQuestao(
        int questionarioId = 1,
        int ordem = 1,
        string nome = "Questao Teste",
        string observacao = "",
        bool obrigatorio = true,
        TipoQuestao tipo = TipoQuestao.Texto,
        string opcionais = "",
        bool somenteLeitura = false,
        int dimensao = 1,
        int? grupoQuestoesId = null,
        int? tamanho = null,
        string? mascara = null,
        string? placeHolder = null,
        string? nomeComponente = null,
        int? id = null)
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

        // Usa reflex�o para definir o Id se fornecido
        if (id.HasValue)
        {
            var idProp = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("Id");
            idProp?.SetValue(questao, id.Value);
        }

        return questao;
    }

    [Fact]
    public async Task ExecutarAsync_QuestaoExiste_DeveExcluirERetornarTrue()
    {
        const long id = 1;
        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: null,
            ordem: 1,
            nome: "Questao Teste",
            observacao: string.Empty,
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: string.Empty,
            somenteLeitura: false,
            dimensao: 1,
            tamanho: null,
            mascara: null,
            placeHolder: null,
            nomeComponente: null,
            id: (int)id
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.RemoverLogico(id,null ,_cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(id, null,_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuestaoNaoExiste_DeveRetornarFalse()
    {
        const long id = 999;

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.Questao?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(It.IsAny<long>(), null,It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaVerificacao_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(It.IsAny<long>(), null,It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaExclusao_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);
        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: null,
            ordem: 1,
            nome: "Questao Teste",
            observacao: string.Empty,
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: string.Empty,
            somenteLeitura: false,
            dimensao: 1,
            tamanho: null,
            mascara: null,
            placeHolder: null,
            nomeComponente: null,
            id: (int)id
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.RemoverLogico(id, null,cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(id, null,cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoObter_DevePropararExcecao()
    {
        const long id = 1;

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao obter quest�o"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao obter quest�o", exception.Message);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(It.IsAny<long>(), null,It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoExcluir_DevePropararExcecao()
    {
        const long id = 1;
        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: null,
            ordem: 1,
            nome: "Questao Teste",
            observacao: string.Empty,
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: string.Empty,
            somenteLeitura: false,
            dimensao: 1,
            tamanho: null,
            mascara: null,
            placeHolder: null,
            nomeComponente: null,
            id: (int)id
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.RemoverLogico(id, null,_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao excluir quest�o"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao excluir quest�o", exception.Message);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(id, null,_cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public async Task ExecutarAsync_ComDiferentesIds_DeveChamarRepositorioComIdCorreto(long id)
    {
        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: null,
            ordem: 1,
            nome: "Questao Teste",
            observacao: string.Empty,
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: string.Empty,
            somenteLeitura: false,
            dimensao: 1,
            tamanho: null,
            mascara: null,
            placeHolder: null,
            nomeComponente: null,
            id: (int)id
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.RemoverLogico(id, null,_cancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(id, _cancellationToken);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(id, null,_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDelegarExclusaoParaRepositorio()
    {
        const long id = 5;
        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: null,
            ordem: 1,
            nome: "Questao Teste",
            observacao: string.Empty,
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: string.Empty,
            somenteLeitura: false,
            dimensao: 1,
            tamanho: null,
            mascara: null,
            placeHolder: null,
            nomeComponente: null,
            id: (int)id
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.RemoverLogico(id, null,_cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(id, null,_cancellationToken), Times.Once);
        _repositorioQuestaoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioExcluirRetornaFalse_DeveRetornarFalse()
    {
        const long id = 1;
        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: null,
            ordem: 1,
            nome: "Questao Teste",
            observacao: string.Empty,
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: string.Empty,
            somenteLeitura: false,
            dimensao: 1,
            tamanho: null,
            mascara: null,
            placeHolder: null,
            nomeComponente: null,
            id: (int)id
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.RemoverLogico(id, null,_cancellationToken))
            .ReturnsAsync(0);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.RemoverLogico(id, null,_cancellationToken), Times.Once);
    }
}