using Moq;
using SME.Sondagem.Aplicacao.UseCases.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Proficiencia;

public class ExcluirProficienciaUseCaseTeste
{
    private readonly Mock<IRepositorioProficiencia> _repositorioProficienciaMock;
    private readonly ExcluirProficienciaUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ExcluirProficienciaUseCaseTeste()
    {
        _repositorioProficienciaMock = new Mock<IRepositorioProficiencia>();
        _useCase = new ExcluirProficienciaUseCase(_repositorioProficienciaMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_ProficienciaExiste_DeveExcluirERetornarTrue()
    {
        const int id = 1;
        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Proficiência Teste", 1) 
        { 
            Id = id 
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.RemoverLogico(id, null,_cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(id, null, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ProficienciaNaoExiste_DeveRetornarFalse()
    {
        const int id = 999;

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Proficiencia?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(It.IsAny<long>(),null, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaVerificacao_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(It.IsAny<long>(), null, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenNaExclusao_DevePropararExcecao()
    {
        const int id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);
        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Proficiência Teste", 1) 
        { 
            Id = id 
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.RemoverLogico(id, null, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(id, null, cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoObter_DevePropararExcecao()
    {
        const long id = 1;

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao obter proficiência"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao obter proficiência", exception.Message);
        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(It.IsAny<long>(), null, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalhaAoExcluir_DevePropararExcecao()
    {
        const int id = 1;
        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Proficiência Teste", 1) 
        { 
            Id = id 
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.RemoverLogico(id, null, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao excluir proficiência"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao excluir proficiência", exception.Message);
        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(id,null, _cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public async Task ExecutarAsync_ComDiferentesIds_DeveChamarRepositorioComIdCorreto(long id)
    {
        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Proficiência Teste", 1) 
        { 
            Id = (int)id 
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.RemoverLogico(id,null, _cancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(id, _cancellationToken);

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(id,null, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDelegarExclusaoParaRepositorio()
    {
        const int id = 5;
        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Proficiência Teste", 1) 
        { 
            Id = id 
        };
        
        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.RemoverLogico(id, null,_cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.True(resultado);
        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(id, null, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioExcluirRetornaFalse_DeveRetornarFalse()
    {
        const int id = 1;
        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Proficiência Teste", 1) 
        { 
            Id = id 
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.RemoverLogico(id,null, _cancellationToken))
            .ReturnsAsync(0);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.False(resultado);
        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.RemoverLogico(id, null, _cancellationToken), Times.Once);
    }
}