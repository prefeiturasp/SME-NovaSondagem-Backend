using Moq;
using SME.Sondagem.Aplicacao.UseCases.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Ciclo;

public class ObterCicloPorIdUseCaseTeste
{
    private readonly Mock<IRepositorioCiclo> _repositorioCicloMock;
    private readonly ObterCicloPorIdUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterCicloPorIdUseCaseTeste()
    {
        _repositorioCicloMock = new Mock<IRepositorioCiclo>();
        _useCase = new ObterCicloPorIdUseCase(_repositorioCicloMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_CicloExiste_DeveRetornarCicloDto()
    {
        const long id = 1;
        var ciclo = new SME.Sondagem.Dominio.Entidades.Ciclo(2, "Ciclo Teste")
        {
            Id = (int)id,
            CriadoEm = DateTime.Now,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001",
            AlteradoEm = DateTime.Now.AddHours(1),
            AlteradoPor = "Usuario2",
            AlteradoRF = "RF002"
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(ciclo);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Ciclo Teste", resultado.DescCiclo);
        Assert.Equal(2, resultado.CodCicloEnsinoEol);
        Assert.Equal("Usuario1", resultado.CriadoPor);
        Assert.Equal("RF001", resultado.CriadoRF);
        Assert.Equal(ciclo.CriadoEm, resultado.CriadoEm);
        Assert.Equal(ciclo.AlteradoEm, resultado.AlteradoEm);
        Assert.Equal("Usuario2", resultado.AlteradoPor);
        Assert.Equal("RF002", resultado.AlteradoRF);

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_CicloNaoExiste_DeveRetornarNull()
    {
        const long id = 999;

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Ciclo?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.Null(resultado);

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComParametrosCorretos()
    {
        const long id = 42;
        var cancellationTokenCustom = new CancellationToken(false);

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCustom))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Ciclo?)null);

        await _useCase.ExecutarAsync(id, cancellationTokenCustom);

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
    }
}