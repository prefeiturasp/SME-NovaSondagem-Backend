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

    [Fact]
    public async Task ExecutarAsync_QuestaoExiste_DeveExcluirERetornarTrue()
    {
        const long id = 1;
        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = null,
            Ordem = 1,
            Nome = "Questao Teste",
            Observacao = string.Empty,
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = string.Empty,
            SomenteLeitura = false,
            Dimensao = 1,
            Tamanho = null,
            Mascara = null,
            PlaceHolder = null,
            NomeComponente = null,
            Id = (int)id
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
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
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
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
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaExclusao_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);
        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = null,
            Ordem = 1,
            Nome = "Questao Teste",
            Observacao = string.Empty,
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = string.Empty,
            SomenteLeitura = false,
            Dimensao = 1,
            Tamanho = null,
            Mascara = null,
            PlaceHolder = null,
            NomeComponente = null,
            Id = (int)id
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.ExcluirAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(id, cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoObter_DevePropararExcecao()
    {
        const long id = 1;

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao obter questão"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao obter questão", exception.Message);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoExcluir_DevePropararExcecao()
    {
        const long id = 1;
        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = null,
            Ordem = 1,
            Nome = "Questao Teste",
            Observacao = string.Empty,
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = string.Empty,
            SomenteLeitura = false,
            Dimensao = 1,
            Tamanho = null,
            Mascara = null,
            PlaceHolder = null,
            NomeComponente = null,
            Id = (int)id
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao excluir questão"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao excluir questão", exception.Message);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public async Task ExecutarAsync_ComDiferentesIds_DeveChamarRepositorioComIdCorreto(long id)
    {
        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = null,
            Ordem = 1,
            Nome = "Questao Teste",
            Observacao = string.Empty,
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = string.Empty,
            SomenteLeitura = false,
            Dimensao = 1,
            Tamanho = null,
            Mascara = null,
            PlaceHolder = null,
            NomeComponente = null,
            Id = (int)id
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        await _useCase.ExecutarAsync(id, _cancellationToken);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDelegarExclusaoParaRepositorio()
    {
        const long id = 5;
        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = null,
            Ordem = 1,
            Nome = "Questao Teste",
            Observacao = string.Empty,
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = string.Empty,
            SomenteLeitura = false,
            Dimensao = 1,
            Tamanho = null,
            Mascara = null,
            PlaceHolder = null,
            NomeComponente = null,
            Id = (int)id
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioExcluirRetornaFalse_DeveRetornarFalse()
    {
        const long id = 1;
        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = null,
            Ordem = 1,
            Nome = "Questao Teste",
            Observacao = string.Empty,
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = string.Empty,
            SomenteLeitura = false,
            Dimensao = 1,
            Tamanho = null,
            Mascara = null,
            PlaceHolder = null,
            NomeComponente = null,
            Id = (int)id
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }
}