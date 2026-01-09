using Moq;
using SME.Sondagem.Aplicacao.UseCases.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Bimestre;

public class ExcluirBimestreUseCaseTeste
{
    private readonly Mock<IRepositorioBimestre> _repositorioBimestreMock;
    private readonly ExcluirBimestreUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ExcluirBimestreUseCaseTeste()
    {
        _repositorioBimestreMock = new Mock<IRepositorioBimestre>();
        _useCase = new ExcluirBimestreUseCase(_repositorioBimestreMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_BimestreExiste_DeveExcluirERetornarTrue()
    {
        const int id = 1;
        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Bimestre Teste") 
        { 
            Id = id 
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_BimestreNaoExiste_DeveRetornarFalse()
    {
        const int id = 999;

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((Dominio.Entidades.Bimestre?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaVerificacao_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaExclusao_DevePropararExcecao()
    {
        const int id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);
        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Bimestre Teste") 
        { 
            Id = id 
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.ExcluirAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(id, cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoObter_DevePropararExcecao()
    {
        const long id = 1;

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao obter proficiência"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao obter proficiência", exception.Message);
        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoExcluir_DevePropararExcecao()
    {
        const int id = 1;
        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Bimestre Teste") 
        { 
            Id = id 
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao excluir proficiência"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao excluir proficiência", exception.Message);
        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public async Task ExecutarAsync_ComDiferentesIds_DeveChamarRepositorioComIdCorreto(long id)
    {
        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Bimestre Teste") 
        { 
            Id = (int)id 
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        await _useCase.ExecutarAsync(id, _cancellationToken);

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDelegarExclusaoParaRepositorio()
    {
        const int id = 5;
        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Bimestre Teste") 
        { 
            Id = id 
        };
        
        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioExcluirRetornaFalse_DeveRetornarFalse()
    {
        const int id = 1;
        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Bimestre Teste") 
        { 
            Id = id 
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }
}