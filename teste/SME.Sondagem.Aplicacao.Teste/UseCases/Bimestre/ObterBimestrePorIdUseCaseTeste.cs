using Moq;
using SME.Sondagem.Aplicacao.UseCases.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Bimestre;

public class ObterBimestrePorIdUseCaseTeste
{
    private readonly Mock<IRepositorioBimestre> _repositorioBimestreMock;
    private readonly ObterBimestrePorIdUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterBimestrePorIdUseCaseTeste()
    {
        _repositorioBimestreMock = new Mock<IRepositorioBimestre>();
        _useCase = new ObterBimestrePorIdUseCase(_repositorioBimestreMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_BimestreExiste_DeveRetornarBimestreDto()
    {
        const int id = 1;
        var bimestre = new Dominio.Entidades.Bimestre(2, "Bimestre Teste")
        {
            Id = (int)id,
            CriadoEm = DateTime.Now,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001",
            AlteradoEm = DateTime.Now.AddHours(1),
            AlteradoPor = "Usuario2",
            AlteradoRF = "RF002"
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestre);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Bimestre Teste", resultado.Descricao);
        Assert.Equal(2, resultado.CodBimestreEnsinoEol);
        Assert.Equal("Usuario1", resultado.CriadoPor);
        Assert.Equal("RF001", resultado.CriadoRF);
        Assert.Equal(bimestre.CriadoEm, resultado.CriadoEm);
        Assert.Equal(bimestre.AlteradoEm, resultado.AlteradoEm);
        Assert.Equal("Usuario2", resultado.AlteradoPor);
        Assert.Equal("RF002", resultado.AlteradoRF);

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_BimestreNaoExiste_DeveRetornarNull()
    {
        const long id = 999;

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((Dominio.Entidades.Bimestre?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.Null(resultado);

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComParametrosCorretos()
    {
        const long id = 42;
        var cancellationTokenCustom = new CancellationToken(false);

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCustom))
            .ReturnsAsync((Dominio.Entidades.Bimestre?)null);

        await _useCase.ExecutarAsync(id, cancellationTokenCustom);

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
    }
}