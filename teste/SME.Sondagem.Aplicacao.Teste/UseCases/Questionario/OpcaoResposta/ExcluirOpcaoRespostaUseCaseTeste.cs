using Moq;
using SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.OpcaoResposta;

public class ExcluirOpcaoRespostaUseCaseTeste
{
    private readonly Mock<IRepositorioOpcaoResposta> _repositorioOpcaoRespostaMock;
    private readonly ExcluirOpcaoRespostaUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ExcluirOpcaoRespostaUseCaseTeste()
    {
        _repositorioOpcaoRespostaMock = new Mock<IRepositorioOpcaoResposta>();
        _useCase = new ExcluirOpcaoRespostaUseCase(_repositorioOpcaoRespostaMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_OpcaoRespostaExiste_DeveExcluirERetornarTrue()
    {
        const long id = 1;
        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção A",
            "Legenda A",
            "#FFFFFF",
            "#000000")
        {
            Id = (int)id
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.RemoverLogico(id, null, default))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(id, It.IsAny<string>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_OpcaoRespostaNaoExiste_DeveRetornarFalse()
    {
        const long id = 999;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(It.IsAny<long>(), It.IsAny<string>(),It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaVerificacao_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaExclusao_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);
        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção A",
            "Legenda A",
            "#FFFFFF",
            "#000000")
        {
            Id = (int)id
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.RemoverLogico(id, It.IsAny<string>(), cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(id, It.IsAny<string>(), cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoObter_DevePropararExcecao()
    {
        const long id = 1;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao obter opção de resposta"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao obter opção de resposta", exception.Message);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(It.IsAny<long>(),It.IsAny<string>() , It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoExcluir_DevePropararExcecao()
    {
        const long id = 1;
        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção A",
            "Legenda A",
            "#FFFFFF",
            "#000000")
        {
            Id = (int)id
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.RemoverLogico(id,null, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao excluir opção de resposta"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao excluir opção de resposta", exception.Message);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(id, It.IsAny<string>(), _cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public async Task ExecutarAsync_ComDiferentesIds_DeveChamarRepositorioComIdCorreto(long id)
    {
        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção A",
            "Legenda A",
            "#FFFFFF",
            "#000000")
        {
            Id = (int)id
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.RemoverLogico(id,null, _cancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(id, _cancellationToken);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(id, null, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDelegarExclusaoParaRepositorio()
    {
        const long id = 5;
        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção A",
            "Legenda A",
            "#FFFFFF",
            "#000000")
        {
            Id = (int)id
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.RemoverLogico(id,null, _cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(id, null, _cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioExcluirRetornaFalse_DeveRetornarFalse()
    {
        const long id = 1;
        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção A",
            "Legenda A",
            "#FFFFFF",
            "#000000")
        {
            Id = (int)id
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.RemoverLogico(id, null,_cancellationToken))
            .ReturnsAsync(0);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.RemoverLogico(id, null, _cancellationToken), Times.Once);
    }
}