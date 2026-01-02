using Moq;
using SME.Sondagem.Aplicacao.UseCases.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Ciclo;

public class CriarCicloUseCaseTeste
{
    private readonly Mock<IRepositorioCiclo> _repositorioCicloMock;
    private readonly CriarCicloUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public CriarCicloUseCaseTeste()
    {
        _repositorioCicloMock = new Mock<IRepositorioCiclo>();
        _useCase = new CriarCicloUseCase(_repositorioCicloMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveCriarCicloERetornarId()
    {
        var cicloDto = new CicloDto
        {
            DescCiclo = "Nova Proficiência",
            CodCicloEnsinoEol = 1,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001"
        };

        const long expectedId = 123;

        _repositorioCicloMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), _cancellationToken))
            .ReturnsAsync(expectedId);

        var resultado = await _useCase.ExecutarAsync(cicloDto, _cancellationToken);

        Assert.Equal(expectedId, resultado);

        _repositorioCicloMock.Verify(x => x.CriarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Ciclo>(p => 
                p.DescCiclo == cicloDto.DescCiclo && 
                p.CodCicloEnsinoEol == cicloDto.CodCicloEnsinoEol),
            _cancellationToken), Times.Once);
    }    

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropararExcecao()
    {
        var cicloDto = new CicloDto
        {
            DescCiclo = "Teste",
            CodCicloEnsinoEol = 1
        };

        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioCicloMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(cicloDto, cancellationTokenCancelado));
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        var cicloDto = new CicloDto
        {
            DescCiclo = "Proficiência Teste",
            CodCicloEnsinoEol = 1
        };

        _repositorioCicloMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(cicloDto, _cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
    }

    [Fact]
    public async Task ExecutarAsync_DeveCriarEntidadeComParametrosCorretos()
    {
        var cicloDto = new CicloDto
        {
            DescCiclo = "Proficiência Específica",
            CodCicloEnsinoEol = 5,
            CriadoPor = "Testador",
            CriadoRF = "RF999"
        };

        SME.Sondagem.Dominio.Entidades.Ciclo? cicloCapturada = null;
        _repositorioCicloMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), _cancellationToken))
            .Callback<SME.Sondagem.Dominio.Entidades.Ciclo, CancellationToken>((p, ct) => cicloCapturada = p)
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(cicloDto, _cancellationToken);

        Assert.NotNull(cicloCapturada);
        Assert.Equal("Proficiência Específica", cicloCapturada.DescCiclo);
        Assert.Equal(5, cicloCapturada.CodCicloEnsinoEol);
    }

    [Fact]
    public async Task ExecutarAsync_ComDiferentesCancellationTokens_DevePropararParaRepositorio()
    {
        var cicloDto = new CicloDto
        {
            DescCiclo = "Teste",
            CodCicloEnsinoEol = 1
        };

        var customCancellationToken = new CancellationTokenSource().Token;

        _repositorioCicloMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), customCancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(cicloDto, customCancellationToken);

        _repositorioCicloMock.Verify(x => x.CriarAsync(
            It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), 
            customCancellationToken), Times.Once);
    }
}