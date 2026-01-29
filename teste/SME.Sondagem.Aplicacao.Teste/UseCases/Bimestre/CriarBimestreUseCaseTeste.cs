using Moq;
using SME.Sondagem.Aplicacao.UseCases.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Bimestre;

public class CriarBimestreUseCaseTeste
{
    private readonly Mock<IRepositorioBimestre> _repositorioBimestreMock;
    private readonly CriarBimestreUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public CriarBimestreUseCaseTeste()
    {
        _repositorioBimestreMock = new Mock<IRepositorioBimestre>();
        _useCase = new CriarBimestreUseCase(_repositorioBimestreMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveCriarBimestreERetornarId()
    {
        var bimestreDto = new BimestreDto
        {
            Descricao = "Nova Proficiência",
            CodBimestreEnsinoEol = 1,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001"
        };

        const long expectedId = 123;

        _repositorioBimestreMock
            .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), _cancellationToken))
            .ReturnsAsync(expectedId);

        var resultado = await _useCase.ExecutarAsync(bimestreDto, _cancellationToken);

        Assert.Equal(expectedId, resultado);

        _repositorioBimestreMock.Verify(x => x.SalvarAsync(
            It.Is<Dominio.Entidades.Bimestre>(p => 
                p.Descricao == bimestreDto.Descricao && 
                p.CodBimestreEnsinoEol == bimestreDto.CodBimestreEnsinoEol),
            _cancellationToken), Times.Once);
    }    

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropararExcecao()
    {
        var bimestreDto = new BimestreDto
        {
            Descricao = "Teste",
            CodBimestreEnsinoEol = 1
        };

        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioBimestreMock
            .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(bimestreDto, cancellationTokenCancelado));
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        var bimestreDto = new BimestreDto
        {
            Descricao = "Proficiência Teste",
            CodBimestreEnsinoEol = 1
        };

        _repositorioBimestreMock
            .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(bimestreDto, _cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
    }

    [Fact]
    public async Task ExecutarAsync_DeveCriarEntidadeComParametrosCorretos()
    {
        var bimestreDto = new BimestreDto
        {
            Descricao = "Proficiência Específica",
            CodBimestreEnsinoEol = 5,
            CriadoPor = "Testador",
            CriadoRF = "RF999"
        };

        Dominio.Entidades.Bimestre? bimestreCapturada = null;
        _repositorioBimestreMock
            .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), _cancellationToken))
            .Callback<Dominio.Entidades.Bimestre, CancellationToken>((p, ct) => bimestreCapturada = p)
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(bimestreDto, _cancellationToken);

        Assert.NotNull(bimestreCapturada);
        Assert.Equal("Proficiência Específica", bimestreCapturada.Descricao);
        Assert.Equal(5, bimestreCapturada.CodBimestreEnsinoEol);
    }

    [Fact]
    public async Task ExecutarAsync_ComDiferentesCancellationTokens_DevePropararParaRepositorio()
    {
        var bimestreDto = new BimestreDto
        {
            Descricao = "Teste",
            CodBimestreEnsinoEol = 1
        };

        var customCancellationToken = new CancellationTokenSource().Token;

        _repositorioBimestreMock
            .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), customCancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(bimestreDto, customCancellationToken);

        _repositorioBimestreMock.Verify(x => x.SalvarAsync(
            It.IsAny<Dominio.Entidades.Bimestre>(), 
            customCancellationToken), Times.Once);
    }
}