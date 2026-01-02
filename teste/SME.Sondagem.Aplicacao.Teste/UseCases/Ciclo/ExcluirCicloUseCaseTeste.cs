using Moq;
using SME.Sondagem.Aplicacao.UseCases.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Ciclo;

public class ExcluirCicloUseCaseTeste
{
    private readonly Mock<IRepositorioCiclo> _repositorioCicloMock;
    private readonly ExcluirCicloUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ExcluirCicloUseCaseTeste()
    {
        _repositorioCicloMock = new Mock<IRepositorioCiclo>();
        _useCase = new ExcluirCicloUseCase(_repositorioCicloMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_CicloExiste_DeveExcluirERetornarTrue()
    {
        const int id = 1;
        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Ciclo Teste") 
        { 
            Id = id 
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_CicloNaoExiste_DeveRetornarFalse()
    {
        const int id = 999;

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Ciclo?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaVerificacao_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaExclusao_DevePropararExcecao()
    {
        const int id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);
        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Ciclo Teste") 
        { 
            Id = id 
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.ExcluirAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(id, cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoObter_DevePropararExcecao()
    {
        const long id = 1;

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao obter proficiência"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao obter proficiência", exception.Message);
        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoExcluir_DevePropararExcecao()
    {
        const int id = 1;
        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Ciclo Teste") 
        { 
            Id = id 
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao excluir proficiência"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao excluir proficiência", exception.Message);
        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public async Task ExecutarAsync_ComDiferentesIds_DeveChamarRepositorioComIdCorreto(long id)
    {
        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Ciclo Teste") 
        { 
            Id = (int)id 
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        await _useCase.ExecutarAsync(id, _cancellationToken);

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDelegarExclusaoParaRepositorio()
    {
        const int id = 5;
        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Ciclo Teste") 
        { 
            Id = id 
        };
        
        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioExcluirRetornaFalse_DeveRetornarFalse()
    {
        const int id = 1;
        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Ciclo Teste") 
        { 
            Id = id 
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.ExcluirAsync(id, _cancellationToken), Times.Once);
    }
}