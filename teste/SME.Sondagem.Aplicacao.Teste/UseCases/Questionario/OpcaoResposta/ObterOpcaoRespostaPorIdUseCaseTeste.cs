using Moq;
using SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.OpcaoResposta;

public class ObterOpcaoRespostaPorIdUseCaseTeste
{
    private readonly Mock<IRepositorioOpcaoResposta> _repositorioOpcaoRespostaMock;
    private readonly ObterOpcaoRespostaPorIdUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterOpcaoRespostaPorIdUseCaseTeste()
    {
        _repositorioOpcaoRespostaMock = new Mock<IRepositorioOpcaoResposta>();
        _useCase = new ObterOpcaoRespostaPorIdUseCase(_repositorioOpcaoRespostaMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_OpcaoRespostaExiste_DeveRetornarOpcaoRespostaDto()
    {
        const long id = 1;
        var dataCriacao = DateTime.Now;
        var dataAlteracao = DateTime.Now.AddHours(1);

        var opcaoResposta = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção A - Resposta correta",
            "Legenda explicativa",
            "#00FF00",
            "#000000")
        {
            Id = (int)id,
            CriadoEm = dataCriacao,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001",
            AlteradoEm = dataAlteracao,
            AlteradoPor = "Usuario2",
            AlteradoRF = "RF002"
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(opcaoResposta);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal("Opção A - Resposta correta", resultado.DescricaoOpcaoResposta);
        Assert.Equal("Legenda explicativa", resultado.Legenda);
        Assert.Equal("#00FF00", resultado.CorFundo);
        Assert.Equal("#000000", resultado.CorTexto);       

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_OpcaoRespostaNaoExiste_DeveRetornarNull()
    {
        const long id = 999;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.Null(resultado);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComParametrosCorretos()
    {
        const long id = 42;
        var cancellationTokenCustom = new CancellationToken(false);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCustom))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta?)null);

        await _useCase.ExecutarAsync(id, cancellationTokenCustom);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_OpcaoRespostaComCamposOpcionaisNulos_DeveRetornarDtoComCamposNulos()
    {
        const long id = 2;
        var dataCriacao = DateTime.Now;

        var opcaoResposta = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção B",
            null,
            null,
            null)
        {
            Id = (int)id,
            CriadoEm = dataCriacao,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001"
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(opcaoResposta);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal("Opção B", resultado.DescricaoOpcaoResposta);
        Assert.Null(resultado.Legenda);
        Assert.Null(resultado.CorFundo);
        Assert.Null(resultado.CorTexto);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        const long id = 1;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao obter opção de resposta"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(id, _cancellationToken));

        Assert.Equal("Erro ao obter opção de resposta", exception.Message);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public async Task ExecutarAsync_ComDiferentesIds_DeveChamarRepositorioComIdCorreto(long id)
    {
        var opcaoResposta = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Opção Teste",
            "Legenda",
            "#FFFFFF",
            "#000000")
        {
            Id = (int)id,
            CriadoEm = DateTime.Now,
            CriadoPor = "Usuario",
            CriadoRF = "RF000"
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(opcaoResposta);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.NotNull(resultado);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }
}