using Moq;
using SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.OpcaoResposta;

public class CriarOpcaoRespostaUseCaseTeste
{
    private readonly Mock<IRepositorioOpcaoResposta> _repositorioOpcaoRespostaMock;
    private readonly CriarOpcaoRespostaUseCase _useCase;

    public CriarOpcaoRespostaUseCaseTeste()
    {
        _repositorioOpcaoRespostaMock = new Mock<IRepositorioOpcaoResposta>();
        _useCase = new CriarOpcaoRespostaUseCase(_repositorioOpcaoRespostaMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveCriarOpcaoRespostaERetornarId()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Sim",
            Legenda = "Op��o positiva",
            CorFundo = "#00FF00",
            CorTexto = "#FFFFFF"
        };

        const long expectedId = 123;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var resultado = await _useCase.ExecutarAsync(opcaoRespostaDto);

        Assert.Equal(expectedId, resultado);

        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(o =>
                o.DescricaoOpcaoResposta == opcaoRespostaDto.DescricaoOpcaoResposta &&
                o.Legenda == opcaoRespostaDto.Legenda &&
                o.CorFundo == opcaoRespostaDto.CorFundo &&
                o.CorTexto == opcaoRespostaDto.CorTexto),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComLegendaNula_DeveCriarCorretamente()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "N�o",
            Legenda = null,
            CorFundo = "#FF0000",
            CorTexto = "#FFFFFF"
        };

        const long expectedId = 456;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var resultado = await _useCase.ExecutarAsync(opcaoRespostaDto);

        Assert.Equal(expectedId, resultado);

        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(o =>
                o.Legenda == null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCoresNulas_DeveCriarCorretamente()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Talvez",
            Legenda = "Op��o neutra",
            CorFundo = null,
            CorTexto = null
        };

        const long expectedId = 789;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var resultado = await _useCase.ExecutarAsync(opcaoRespostaDto);

        Assert.Equal(expectedId, resultado);

        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(o =>
                o.CorFundo == null &&
                o.CorTexto == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComDiferentesDescricoes_DeveCriarCorretamente()
    {
        var descricoes = new[] { "Sim", "N�o", "Talvez", "N�o se aplica", "Prefiro n�o responder" };
        long idEsperado = 100;

        foreach (var descricao in descricoes)
        {
            var opcaoRespostaDto = new OpcaoRespostaDto
            {
                DescricaoOpcaoResposta = descricao,
                Legenda = $"Legenda {descricao}",
                CorFundo = "#CCCCCC",
                CorTexto = "#000000"
            };

            _repositorioOpcaoRespostaMock
                .Setup(x => x.SalvarAsync(
                    It.Is<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(o => o.DescricaoOpcaoResposta == descricao),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(idEsperado++);

            var resultado = await _useCase.ExecutarAsync(opcaoRespostaDto);

            Assert.True(resultado > 0);
        }

        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(),
            It.IsAny<CancellationToken>()), Times.Exactly(descricoes.Length));
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropararExcecao()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Teste",
            Legenda = "Teste",
            CorFundo = "#000000",
            CorTexto = "#FFFFFF"
        };

        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(opcaoRespostaDto, cancellationTokenCancelado));
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Op��o Teste",
            Legenda = "Legenda Teste",
            CorFundo = "#AAAAAA",
            CorTexto = "#BBBBBB"
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(opcaoRespostaDto));

        Assert.Equal("Erro do repositório", exception.Message);
    }

    [Fact]
    public async Task ExecutarAsync_DeveCriarEntidadeComTodosOsParametrosCorretos()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "opção de Resposta Especifica",
            Legenda = "Legenda detalhada da opção",
            CorFundo = "#FF5733",
            CorTexto = "#FFFFFF"
        };

        SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta? opcaoRespostaCapturada = null;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()))
            .Callback<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta, CancellationToken>((o, ct) => opcaoRespostaCapturada = o)
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(opcaoRespostaDto);

        Assert.NotNull(opcaoRespostaCapturada);
        Assert.Equal("Opção de Resposta Específica", opcaoRespostaCapturada.DescricaoOpcaoResposta);
        Assert.Equal("Legenda detalhada da opção", opcaoRespostaCapturada.Legenda);
        Assert.Equal("#FF5733", opcaoRespostaCapturada.CorFundo);
        Assert.Equal("#FFFFFF", opcaoRespostaCapturada.CorTexto);
    }

    [Fact]
    public async Task ExecutarAsync_ComDiferentesCancellationTokens_DevePropararParaRepositorio()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Teste",
            Legenda = "Teste Legenda",
            CorFundo = "#123456",
            CorTexto = "#ABCDEF"
        };

        var customCancellationToken = new CancellationTokenSource().Token;

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), customCancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(opcaoRespostaDto, customCancellationToken);

        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(),
                 customCancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComDescricaoVazia_DeveCriarCorretamente()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = string.Empty,
            Legenda = "Op��o sem descri��o",
            CorFundo = "#FFFFFF",
            CorTexto = "#000000"
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(),  It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(opcaoRespostaDto);

        Assert.True(resultado > 0);
        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(o => o.DescricaoOpcaoResposta == string.Empty),
             It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComDescricaoLonga_DeveCriarCorretamente()
    {
        var descricaoLonga = new string('A', 500);
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = descricaoLonga,
            Legenda = "Legenda para descri��o longa",
            CorFundo = "#EEEEEE",
            CorTexto = "#111111"
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(),  It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(opcaoRespostaDto);

        Assert.True(resultado > 0);
        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(o => o.DescricaoOpcaoResposta == descricaoLonga),
             It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComDiferentesCoresFundo_DeveCriarCorretamente()
    {
        var cores = new[] { "#FF0000", "#00FF00", "#0000FF", "#FFFF00", "#FF00FF" };
        long idEsperado = 200;

        foreach (var cor in cores)
        {
            var opcaoRespostaDto = new OpcaoRespostaDto
            {
                DescricaoOpcaoResposta = $"Op��o com cor {cor}",
                Legenda = "Legenda",
                CorFundo = cor,
                CorTexto = "#FFFFFF"
            };

            _repositorioOpcaoRespostaMock
                .Setup(x => x.SalvarAsync(
                    It.Is<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(o => o.CorFundo == cor),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(idEsperado++);

            var resultado = await _useCase.ExecutarAsync(opcaoRespostaDto);

            Assert.True(resultado > 0);
        }

        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(),
             It.IsAny<CancellationToken>()), Times.Exactly(cores.Length));
    }

    [Fact]
    public async Task ExecutarAsync_ComTodosCamposNulos_DeveCriarCorretamente()
    {
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Descri��o obrigat�ria",
            Legenda = null,
            CorFundo = null,
            CorTexto = null
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(),  It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(opcaoRespostaDto);

        Assert.True(resultado > 0);
        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(o =>
                o.Legenda == null &&
                o.CorFundo == null &&
                o.CorTexto == null),
             It.IsAny<CancellationToken>()), Times.Once);
    }
}