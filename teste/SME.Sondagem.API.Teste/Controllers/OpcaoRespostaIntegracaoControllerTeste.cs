using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller;

[ExcludeFromCodeCoverage]
public class OpcaoRespostaIntegracaoControllerTeste
{
    private readonly Mock<IObterOpcaoRespostaUseCase> _obterMock;
    private readonly Mock<IObterOpcaoRespostaPorIdUseCase> _obterPorIdMock;
    private readonly Mock<ICriarOpcaoRespostaUseCase> _criarMock;
    private readonly Mock<IAtualizarOpcaoRespostaUseCase> _atualizarMock;
    private readonly Mock<IExcluirOpcaoRespostaUseCase> _excluirMock;

    public OpcaoRespostaIntegracaoControllerTeste()
    {
        _obterMock = new Mock<IObterOpcaoRespostaUseCase>();
        _obterPorIdMock = new Mock<IObterOpcaoRespostaPorIdUseCase>();
        _criarMock = new Mock<ICriarOpcaoRespostaUseCase>();
        _atualizarMock = new Mock<IAtualizarOpcaoRespostaUseCase>();
        _excluirMock = new Mock<IExcluirOpcaoRespostaUseCase>();
    }

    private OpcaoRespostaIntegracaoController CriarController()
        => new(
            _obterMock.Object,
            _obterPorIdMock.Object,
            _criarMock.Object,
            _atualizarMock.Object,
            _excluirMock.Object
        );

    private static OpcaoRespostaDto CriarDto(int id = 1, int ordem = 1, string descricao = "Opção A")
        => new()
        {
            Id = id,
            Ordem = ordem,
            DescricaoOpcaoResposta = descricao,
            Legenda = "Legenda teste",
            CorFundo = "#FFFFFF",
            CorTexto = "#000000"
        };

    private static List<OpcaoRespostaDto> CriarListaDto()
        => new()
        {
            CriarDto(1, 1, "Opção A"),
            CriarDto(2, 2, "Opção B"),
            CriarDto(3, 3, "Opção C")
        };

    #region GET - Obter Todas as Opções

    [Fact(DisplayName = "Get - Deve retornar Ok com lista de opções de resposta")]
    public async Task Get_DeveRetornarOk_ComListaDeOpcoes()
    {
        // Arrange
        var opcoesEsperadas = CriarListaDto();
        _obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(opcoesEsperadas);

        var controller = CriarController();

        // Act
        var result = await controller.Get(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var opcoes = okResult.Value.Should().BeAssignableTo<IEnumerable<OpcaoRespostaDto>>().Subject;
        opcoes.Should().HaveCount(3);
        opcoes.Should().BeEquivalentTo(opcoesEsperadas);

        _obterMock.Verify(x => x.ExecutarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Get - Deve retornar Ok com lista vazia quando não houver opções")]
    public async Task Get_DeveRetornarOk_ComListaVazia()
    {
        // Arrange
        _obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new List<OpcaoRespostaDto>());

        var controller = CriarController();

        // Act
        var result = await controller.Get(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var opcoes = okResult.Value.Should().BeAssignableTo<IEnumerable<OpcaoRespostaDto>>().Subject;
        opcoes.Should().BeEmpty();
    }

    [Fact(DisplayName = "Get - Deve propagar exceção quando UseCase lançar exceção")]
    public async Task Get_DevePropagar_QuandoUseCaseLancarExcecao()
    {
        // Arrange
        _obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                  .ThrowsAsync(new Exception("Erro ao buscar opções"));

        var controller = CriarController();

        // Act
        Func<Task> act = async () => await controller.Get(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Erro ao buscar opções");
    }

    #endregion

    #region GET BY ID - Obter Opção por ID

    [Fact(DisplayName = "GetById - Deve retornar Ok com opção de resposta encontrada")]
    public async Task GetById_DeveRetornarOk_ComOpcaoEncontrada()
    {
        // Arrange
        const int idEsperado = 1;
        var opcaoEsperada = CriarDto(idEsperado);

        _obterPorIdMock.Setup(x => x.ExecutarAsync(idEsperado, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(opcaoEsperada);

        var controller = CriarController();

        // Act
        var result = await controller.GetById(idEsperado, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var opcao = okResult.Value.Should().BeOfType<OpcaoRespostaDto>().Subject;
        opcao.Should().BeEquivalentTo(opcaoEsperada);
        opcao.Id.Should().Be(idEsperado);

        _obterPorIdMock.Verify(x => x.ExecutarAsync(idEsperado, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "GetById - Deve lançar RegraNegocioException quando opção não for encontrada")]
    public async Task GetById_DeveLancarRegraNegocioException_QuandoNaoEncontrado()
    {
        // Arrange
        const long idInexistente = 999;

        _obterPorIdMock.Setup(x => x.ExecutarAsync(idInexistente, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((OpcaoRespostaDto?)null);

        var controller = CriarController();

        // Act
        Func<Task> act = async () => await controller.GetById(idInexistente, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<RegraNegocioException>();
        exception.Which.Message.Should().Contain(idInexistente.ToString());
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        _obterPorIdMock.Verify(x => x.ExecutarAsync(idInexistente, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = "GetById - Deve buscar corretamente diferentes IDs")]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task GetById_DeveBuscarCorretamente_DiferentesIds(int id)
    {
        // Arrange
        var opcaoEsperada = CriarDto(id);

        _obterPorIdMock.Setup(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(opcaoEsperada);

        var controller = CriarController();

        // Act
        var result = await controller.GetById(id, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var opcao = okResult.Value.Should().BeOfType<OpcaoRespostaDto>().Subject;
        opcao.Id.Should().Be(id);
    }

    #endregion

    #region POST - Criar Opção

    [Fact(DisplayName = "Create - Deve retornar Created com ID da opção criada")]
    public async Task Create_DeveRetornarCreated_ComIdDaOpcaoCriada()
    {
        // Arrange
        const long idCriado = 1;
        var dto = CriarDto();

        _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(idCriado);

        var controller = CriarController();

        // Act
        var result = await controller.Create(dto, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.ActionName.Should().Be(nameof(OpcaoRespostaIntegracaoController.GetById));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(idCriado);
        createdResult.Value.Should().Be(idCriado);

        _criarMock.Verify(x => x.ExecutarAsync(
            It.Is<OpcaoRespostaDto>(d => d.DescricaoOpcaoResposta == dto.DescricaoOpcaoResposta),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Create - Deve criar opção com todos os campos preenchidos")]
    public async Task Create_DeveCriarOpcao_ComTodosCamposPreenchidos()
    {
        // Arrange
        var dto = new OpcaoRespostaDto
        {
            Ordem = 5,
            DescricaoOpcaoResposta = "Opção Completa",
            Legenda = "Legenda completa",
            CorFundo = "#FF5733",
            CorTexto = "#FFFFFF"
        };

        _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(1);

        var controller = CriarController();

        // Act
        var result = await controller.Create(dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();

        _criarMock.Verify(x => x.ExecutarAsync(
            It.Is<OpcaoRespostaDto>(d =>
                d.Ordem == 5 &&
                d.DescricaoOpcaoResposta == "Opção Completa" &&
                d.Legenda == "Legenda completa" &&
                d.CorFundo == "#FF5733" &&
                d.CorTexto == "#FFFFFF"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Create - Deve criar opção com campos opcionais nulos")]
    public async Task Create_DeveCriarOpcao_ComCamposOpcionaisNulos()
    {
        // Arrange
        var dto = new OpcaoRespostaDto
        {
            Ordem = 1,
            DescricaoOpcaoResposta = "Opção Simples",
            Legenda = null,
            CorFundo = null,
            CorTexto = null
        };

        _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(1);

        var controller = CriarController();

        // Act
        var result = await controller.Create(dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();

        _criarMock.Verify(x => x.ExecutarAsync(
            It.Is<OpcaoRespostaDto>(d =>
                d.Legenda == null &&
                d.CorFundo == null &&
                d.CorTexto == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Create - Deve propagar exceção quando UseCase lançar exceção")]
    public async Task Create_DevePropagar_QuandoUseCaseLancarExcecao()
    {
        // Arrange
        var dto = CriarDto();

        _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                  .ThrowsAsync(new RegraNegocioException("Erro ao criar opção", StatusCodes.Status400BadRequest));

        var controller = CriarController();

        // Act
        Func<Task> act = async () => await controller.Create(dto, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<RegraNegocioException>();
        exception.Which.Message.Should().Contain("Erro ao criar opção");
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    #endregion

    #region PUT - Atualizar Opção

    [Fact(DisplayName = "Atualizar - Deve retornar Ok com opção atualizada")]
    public async Task Atualizar_DeveRetornarOk_ComOpcaoAtualizada()
    {
        // Arrange
        const int id = 1;
        var dto = CriarDto(id);
        var opcaoAtualizada = CriarDto(id, 2, "Opção Atualizada");

        _atualizarMock.Setup(x => x.ExecutarAsync(id, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(opcaoAtualizada);

        var controller = CriarController();

        // Act
        var result = await controller.Atualizar(id, dto, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var opcao = okResult.Value.Should().BeOfType<OpcaoRespostaDto>().Subject;
        opcao.Should().BeEquivalentTo(opcaoAtualizada);

        _atualizarMock.Verify(x => x.ExecutarAsync(id, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Atualizar - Deve lançar RegraNegocioException quando opção não for encontrada")]
    public async Task Atualizar_DeveLancarRegraNegocioException_QuandoNaoEncontrado()
    {
        // Arrange
        const int idInexistente = 999;
        var dto = CriarDto();

        _atualizarMock.Setup(x => x.ExecutarAsync(idInexistente, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((OpcaoRespostaDto?)null);

        var controller = CriarController();

        // Act
        Func<Task> act = async () => await controller.Atualizar(idInexistente, dto, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<RegraNegocioException>();
        exception.Which.Message.Should().Contain(idInexistente.ToString());
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        _atualizarMock.Verify(x => x.ExecutarAsync(idInexistente, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Atualizar - Deve atualizar todos os campos da opção")]
    public async Task Atualizar_DeveAtualizarTodosCampos_DaOpcao()
    {
        // Arrange
        const int id = 1;
        var dtoAtualizado = new OpcaoRespostaDto
        {
            Id = id,
            Ordem = 10,
            DescricaoOpcaoResposta = "Descrição Atualizada",
            Legenda = "Nova Legenda",
            CorFundo = "#000000",
            CorTexto = "#FFFFFF"
        };

        _atualizarMock.Setup(x => x.ExecutarAsync(id, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(dtoAtualizado);

        var controller = CriarController();

        // Act
        var result = await controller.Atualizar(id, dtoAtualizado, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var opcao = okResult.Value.Should().BeOfType<OpcaoRespostaDto>().Subject;

        opcao.Ordem.Should().Be(10);
        opcao.DescricaoOpcaoResposta.Should().Be("Descrição Atualizada");
        opcao.Legenda.Should().Be("Nova Legenda");
        opcao.CorFundo.Should().Be("#000000");
        opcao.CorTexto.Should().Be("#FFFFFF");
    }

    [Theory(DisplayName = "Atualizar - Deve validar ID correto na atualização")]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task Atualizar_DeveValidarIdCorreto_NaAtualizacao(int id)
    {
        // Arrange
        var dto = CriarDto(id);

        _atualizarMock.Setup(x => x.ExecutarAsync(id, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(dto);

        var controller = CriarController();

        // Act
        var result = await controller.Atualizar(id, dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _atualizarMock.Verify(x => x.ExecutarAsync(
            id,
            It.IsAny<OpcaoRespostaDto>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DELETE - Excluir Opção

    [Fact(DisplayName = "Excluir - Deve retornar NoContent quando exclusão for bem-sucedida")]
    public async Task Excluir_DeveRetornarNoContent_QuandoExclusaoBemSucedida()
    {
        // Arrange
        const int id = 1;

        _excluirMock.Setup(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

        var controller = CriarController();

        // Act
        var result = await controller.Excluir(id, CancellationToken.None);

        // Assert
        var noContentResult = result.Should().BeOfType<NoContentResult>().Subject;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _excluirMock.Verify(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Excluir - Deve lançar RegraNegocioException quando opção não for encontrada")]
    public async Task Excluir_DeveLancarRegraNegocioException_QuandoNaoEncontrado()
    {
        // Arrange
        const int idInexistente = 999;

        _excluirMock.Setup(x => x.ExecutarAsync(idInexistente, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

        var controller = CriarController();

        // Act
        Func<Task> act = async () => await controller.Excluir(idInexistente, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<RegraNegocioException>();
        exception.Which.Message.Should().Contain(idInexistente.ToString());
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        _excluirMock.Verify(x => x.ExecutarAsync(idInexistente, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = "Excluir - Deve excluir corretamente diferentes IDs")]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(100)]
    public async Task Excluir_DeveExcluirCorretamente_DiferentesIds(int id)
    {
        // Arrange
        _excluirMock.Setup(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

        var controller = CriarController();

        // Act
        var result = await controller.Excluir(id, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        _excluirMock.Verify(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Excluir - Deve propagar exceção quando UseCase lançar exceção")]
    public async Task Excluir_DevePropagar_QuandoUseCaseLancarExcecao()
    {
        // Arrange
        const int id = 1;

        _excluirMock.Setup(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception("Erro ao excluir opção"));

        var controller = CriarController();

        // Act
        Func<Task> act = async () => await controller.Excluir(id, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Erro ao excluir opção");
    }

    #endregion

    #region Testes de CancellationToken

    [Fact(DisplayName = "Get - Deve respeitar CancellationToken")]
    public async Task Get_DeveRespeitarCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _obterMock.Setup(x => x.ExecutarAsync(token))
                  .ReturnsAsync(CriarListaDto());

        var controller = CriarController();

        // Act
        await controller.Get(token);

        // Assert
        _obterMock.Verify(x => x.ExecutarAsync(token), Times.Once);
    }

    [Fact(DisplayName = "GetById - Deve respeitar CancellationToken")]
    public async Task GetById_DeveRespeitarCancellationToken()
    {
        // Arrange
        const long id = 1;
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _obterPorIdMock.Setup(x => x.ExecutarAsync(id, token))
                       .ReturnsAsync(CriarDto());

        var controller = CriarController();

        // Act
        await controller.GetById(id, token);

        // Assert
        _obterPorIdMock.Verify(x => x.ExecutarAsync(id, token), Times.Once);
    }

    [Fact(DisplayName = "Create - Deve respeitar CancellationToken")]
    public async Task Create_DeveRespeitarCancellationToken()
    {
        // Arrange
        var dto = CriarDto();
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _criarMock.Setup(x => x.ExecutarAsync(dto, token))
                  .ReturnsAsync(1);

        var controller = CriarController();

        // Act
        await controller.Create(dto, token);

        // Assert
        _criarMock.Verify(x => x.ExecutarAsync(dto, token), Times.Once);
    }

    [Fact(DisplayName = "Atualizar - Deve respeitar CancellationToken")]
    public async Task Atualizar_DeveRespeitarCancellationToken()
    {
        // Arrange
        const int id = 1;
        var dto = CriarDto();
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _atualizarMock.Setup(x => x.ExecutarAsync(id, dto, token))
                      .ReturnsAsync(dto);

        var controller = CriarController();

        // Act
        await controller.Atualizar(id, dto, token);

        // Assert
        _atualizarMock.Verify(x => x.ExecutarAsync(id, dto, token), Times.Once);
    }

    [Fact(DisplayName = "Excluir - Deve respeitar CancellationToken")]
    public async Task Excluir_DeveRespeitarCancellationToken()
    {
        // Arrange
        const int id = 1;
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _excluirMock.Setup(x => x.ExecutarAsync(id, token))
                    .ReturnsAsync(true);

        var controller = CriarController();

        // Act
        await controller.Excluir(id, token);

        // Assert
        _excluirMock.Verify(x => x.ExecutarAsync(id, token), Times.Once);
    }

    #endregion
}
