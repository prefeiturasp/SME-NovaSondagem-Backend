using Moq;
using SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.OpcaoResposta;

public class AtualizarOpcaoRespostaUseCaseTeste
{
    private readonly Mock<IRepositorioOpcaoResposta> _repositorioOpcaoRespostaMock;
    private readonly AtualizarOpcaoRespostaUseCase _useCase;

    public AtualizarOpcaoRespostaUseCaseTeste()
    {
        _repositorioOpcaoRespostaMock = new Mock<IRepositorioOpcaoResposta>();
        _useCase = new AtualizarOpcaoRespostaUseCase(_repositorioOpcaoRespostaMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_OpcaoRespostaNaoExiste_DeveRetornarNull()
    {
        const long id = 999;
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Op��o A",
            Legenda = "Legenda teste",
            CorFundo = "#FFFFFF",
            CorTexto = "#000000"
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta?)null);

        var resultado = await _useCase.ExecutarAsync(id, opcaoRespostaDto);

        Assert.Null(resultado);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_AtualizacaoFalha_DeveRetornarNull()
    {
        const long id = 1;
        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Op��o B Atualizada",
            Legenda = "Nova Legenda",
            CorFundo = "#FF0000",
            CorTexto = "#FFFFFF"
        };

        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Op��o B Original",
            "Legenda Original",
            "#000000",
            "#FFFFFF"
        );

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var resultado = await _useCase.ExecutarAsync(id, opcaoRespostaDto);

        Assert.Null(resultado);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(opcaoRespostaExistente, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_AtualizacaoComSucesso_DeveRetornarOpcaoRespostaDtoCompleto()
    {
        const long id = 1;

        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Op��o C Atualizada",
            Legenda = "Legenda Atualizada",
            CorFundo = "#00FF00",
            CorTexto = "#000000"
        };

        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Op��o C Original",
            "Legenda Original",
            "#0000FF",
            "#FFFFFF"
        );

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, opcaoRespostaDto);

        Assert.NotNull(resultado);
        Assert.Equal("Op��o C Atualizada", resultado.DescricaoOpcaoResposta);
        Assert.Equal("Legenda Atualizada", resultado.Legenda);
        Assert.Equal("#00FF00", resultado.CorFundo);
        Assert.Equal("#000000", resultado.CorTexto);

        // Verifica se os dados foram atualizados na entidade
        Assert.Equal("Op��o C Atualizada", opcaoRespostaExistente.DescricaoOpcaoResposta);
        Assert.Equal("Legenda Atualizada", opcaoRespostaExistente.Legenda);
        Assert.Equal("#00FF00", opcaoRespostaExistente.CorFundo);
        Assert.Equal("#000000", opcaoRespostaExistente.CorTexto);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(opcaoRespostaExistente, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropagaCancellationToken()
    {
        const long id = 1;
        var cancellationToken = new CancellationToken();

        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Op��o D",
            Legenda = "Legenda D",
            CorFundo = "#FFFF00",
            CorTexto = "#000000"
        };

        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Op��o D Original",
            "Legenda Original",
            "#000000",
            "#FFFFFF"
        );

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, opcaoRespostaDto, cancellationToken);

        Assert.NotNull(resultado);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(opcaoRespostaExistente, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCamposNulos_DeveAtualizarCorretamente()
    {
        const long id = 1;

        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Op��o E",
            Legenda = null,
            CorFundo = null,
            CorTexto = null
        };

        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Op��o E Original",
            "Legenda Original",
            "#000000",
            "#FFFFFF"
        );

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, opcaoRespostaDto);

        Assert.NotNull(resultado);
        Assert.Equal("Op��o E", resultado.DescricaoOpcaoResposta);
        Assert.Null(resultado.Legenda);
        Assert.Null(resultado.CorFundo);
        Assert.Null(resultado.CorTexto);

        // Verifica se os campos nulos foram atualizados corretamente
        Assert.Equal("Op��o E", opcaoRespostaExistente.DescricaoOpcaoResposta);
        Assert.Null(opcaoRespostaExistente.Legenda);
        Assert.Null(opcaoRespostaExistente.CorFundo);
        Assert.Null(opcaoRespostaExistente.CorTexto);
    }

    [Fact]
    public async Task ExecutarAsync_DeveAtualizarTodasAsPropriedades()
    {
        const long id = 1;

        var opcaoRespostaDto = new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = "Op��o F Atualizada",
            Legenda = "Legenda F Atualizada",
            CorFundo = "#FF00FF",
            CorTexto = "#00FFFF"
        };

        var opcaoRespostaExistente = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            1,
            "Op��o F Original",
            "Legenda F Original",
            "#123456",
            "#ABCDEF"
        );

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(opcaoRespostaExistente);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, opcaoRespostaDto);

        // Verifica se a atualização foi bem-sucedida
        Assert.NotNull(resultado);
        Assert.Equal("Op��o F Atualizada", resultado.DescricaoOpcaoResposta);
        Assert.Equal("Legenda F Atualizada", resultado.Legenda);
        Assert.Equal("#FF00FF", resultado.CorFundo);
        Assert.Equal("#00FFFF", resultado.CorTexto);

        // Verifica se todas as propriedades da entidade foram atualizadas
        Assert.Equal("Op��o F Atualizada", opcaoRespostaExistente.DescricaoOpcaoResposta);
        Assert.Equal("Legenda F Atualizada", opcaoRespostaExistente.Legenda);
        Assert.Equal("#FF00FF", opcaoRespostaExistente.CorFundo);
        Assert.Equal("#00FFFF", opcaoRespostaExistente.CorTexto);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterPorIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositorioOpcaoRespostaMock.Verify(x => x.SalvarAsync(opcaoRespostaExistente, It.IsAny<CancellationToken>()), Times.Once);
    }
}