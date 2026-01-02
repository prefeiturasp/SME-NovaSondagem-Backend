using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste;

public class QuestaoControllerTeste
{
    private readonly Mock<IObterQuestoesUseCase> _obterQuestoesUseCaseMock;
    private readonly Mock<IObterQuestaoPorIdUseCase> _obterQuestaoPorIdUseCaseMock;
    private readonly Mock<ICriarQuestaoUseCase> _criarQuestaoUseCaseMock;
    private readonly Mock<IAtualizarQuestaoUseCase> _atualizarQuestaoUseCaseMock;
    private readonly Mock<IExcluirQuestaoUseCase> _excluirQuestaoUseCaseMock;
    private readonly Mock<ILogger<QuestaoController>> _loggerMock;
    private readonly QuestaoController _controller;
    private readonly CancellationToken _cancellationToken;

    public QuestaoControllerTeste()
    {
        _obterQuestoesUseCaseMock = new Mock<IObterQuestoesUseCase>();
        _obterQuestaoPorIdUseCaseMock = new Mock<IObterQuestaoPorIdUseCase>();
        _criarQuestaoUseCaseMock = new Mock<ICriarQuestaoUseCase>();
        _atualizarQuestaoUseCaseMock = new Mock<IAtualizarQuestaoUseCase>();
        _excluirQuestaoUseCaseMock = new Mock<IExcluirQuestaoUseCase>();
        _loggerMock = new Mock<ILogger<QuestaoController>>();
        _cancellationToken = CancellationToken.None;

        _controller = new QuestaoController(
            _obterQuestoesUseCaseMock.Object,
            _obterQuestaoPorIdUseCaseMock.Object,
            _criarQuestaoUseCaseMock.Object,
            _atualizarQuestaoUseCaseMock.Object,
            _excluirQuestaoUseCaseMock.Object,
            _loggerMock.Object
        );
    }

    #region Listar Tests

    [Fact]
    public async Task Listar_DeveRetornarOk_ComListaDeQuestoes()
    {
        var questoes = new List<QuestaoDto>
        {
            new QuestaoDto { Id = 1, Nome = "Questao 1" },
            new QuestaoDto { Id = 2, Nome = "Questao 2" }
        };

        _obterQuestoesUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ReturnsAsync(questoes);

        var result = await _controller.Listar(_cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(questoes, okResult.Value);
    }

    [Fact]
    public async Task Listar_OperationCanceledException_DeveRetornarStatus499()
    {
        _obterQuestoesUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Listar(_cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogInformation(_loggerMock, "Requisição de listagem foi cancelada");
    }

    [Fact]
    public async Task Listar_Exception_DeveRetornarStatus500()
    {
        var exception = new Exception("Erro interno");
        _obterQuestoesUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Listar(_cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao listar questões", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogError(_loggerMock, "Erro ao listar questões", exception);
    }

    #endregion

    #region ObterPorId Tests

    [Fact]
    public async Task ObterPorId_DeveRetornarOk_ComQuestao()
    {
        const int id = 1;
        var questao = new QuestaoDto { Id = 1, Nome = "Questao Teste" };

        _obterQuestaoPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(questao);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(questao, okResult.Value);
    }

    [Fact]
    public async Task ObterPorId_QuestaoNaoEncontrado_DeveRetornarNotFound()
    {
        const int id = 999;
        _obterQuestaoPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync((QuestaoDto?)null);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Questão com ID {id} não encontrada", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task ObterPorId_OperationCanceledException_DeveRetornarStatus499()
    {
        const int id = 1;
        _obterQuestaoPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogInformationWithId(_loggerMock, "Requisição de obtenção foi cancelada para ID", id);
    }

    [Fact]
    public async Task ObterPorId_Exception_DeveRetornarStatus500()
    {
        const int id = 1;
        var exception = new Exception("Erro interno");
        _obterQuestaoPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao obter questão", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogErrorWithId(_loggerMock, "Erro ao obter questão", exception, id);
    }

    #endregion

    #region Criar Tests

    [Fact]
    public async Task Criar_DeveRetornarCreated_ComQuestaoCriado()
    {
        var questaoDto = new QuestaoDto { Nome = "Nova Questao" };
        var questaoCriada = new QuestaoDto { Id = 1, Nome = "Nova Questao" };
        const int questaoId = 1;

        _criarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(questaoDto, _cancellationToken))
            .ReturnsAsync(questaoId);

        _obterQuestaoPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(questaoId, _cancellationToken))
            .ReturnsAsync(questaoCriada);

        var result = await _controller.Criar(questaoDto, _cancellationToken);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(nameof(QuestaoController.ObterPorId), createdResult.ActionName);

        var routeValues = createdResult.RouteValues;
        Assert.NotNull(routeValues);
        Assert.True(routeValues.ContainsKey("id"));
        Assert.Equal(questaoId, Convert.ToInt32(routeValues["id"]!));

        var resultValue = Assert.IsType<QuestaoDto>(createdResult.Value);
        Assert.Equal(questaoId, resultValue.Id);
        Assert.Equal("Nova Questao", resultValue.Nome);
    }

    [Fact]
    public async Task Criar_OperationCanceledException_DeveRetornarStatus499()
    {
        var questaoDto = new QuestaoDto { Nome = "Nova Questao" };
        _criarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(questaoDto, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Criar(questaoDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogInformation(_loggerMock, "Requisição de criação foi cancelada");
    }

    [Fact]
    public async Task Criar_ValidationException_DeveRetornarBadRequest()
    {
        var questaoDto = new QuestaoDto { Nome = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        };
        var validationException = new ValidationException(validationFailures);

        _criarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(questaoDto, _cancellationToken))
            .ThrowsAsync(validationException);

        var result = await _controller.Criar(questaoDto, _cancellationToken);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        Assert.NotNull(badRequestResult.Value);
        var mensagemProperty = badRequestResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro de validação", mensagemProperty?.GetValue(badRequestResult.Value));

        var errosProperty = badRequestResult.Value.GetType().GetProperty("erros");
        Assert.NotNull(errosProperty?.GetValue(badRequestResult.Value));
    }

    [Fact]
    public async Task Criar_NegocioException_DeveRetornarStatusDaExcecao()
    {
        var questaoDto = new QuestaoDto { Nome = "Questao Duplicado" };
        var negocioException = new RegraNegocioException("Questao já existe", StatusCodes.Status409Conflict);

        _criarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(questaoDto, _cancellationToken))
            .ThrowsAsync(negocioException);

        var result = await _controller.Criar(questaoDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Questao já existe", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Criar_Exception_DeveRetornarStatus500()
    {
        var questaoDto = new QuestaoDto { Nome = "Nova Questao" };
        var exception = new Exception("Erro interno");

        _criarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(questaoDto, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Criar(questaoDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao criar questão", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogError(_loggerMock, "Erro ao criar questão", exception);
    }

    #endregion

    #region Atualizar Tests

    [Fact]
    public async Task Atualizar_DeveRetornarOk_ComQuestaoAtualizado()
    {
        const int id = 1;
        var questaoDto = new QuestaoDto { Id = id, Nome = "Questao Atualizado" };

        _atualizarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, questaoDto, _cancellationToken))
            .ReturnsAsync(questaoDto);

        var result = await _controller.Atualizar(id, questaoDto, _cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(questaoDto, okResult.Value);
    }

    [Fact]
    public async Task Atualizar_QuestaoNaoEncontrado_DeveRetornarNotFound()
    {
        const int id = 999;
        var questaoDto = new QuestaoDto { Id = id, Nome = "Questao" };

        _atualizarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, questaoDto, _cancellationToken))
            .ReturnsAsync((QuestaoDto?)null);

        var result = await _controller.Atualizar(id, questaoDto, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Questão com ID {id} não encontrada", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task Atualizar_OperationCanceledException_DeveRetornarStatus499()
    {
        const int id = 1;
        var questaoDto = new QuestaoDto { Id = id, Nome = "Questao Atualizado" };

        _atualizarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, questaoDto, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Atualizar(id, questaoDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogInformationWithId(_loggerMock, "Requisição de atualização foi cancelada para ID", id);
    }

    [Fact]
    public async Task Atualizar_ValidationException_DeveRetornarBadRequest()
    {
        const int id = 1;
        var questaoDto = new QuestaoDto { Id = id, Nome = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        };
        var validationException = new ValidationException(validationFailures);

        _atualizarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, questaoDto, _cancellationToken))
            .ThrowsAsync(validationException);

        var result = await _controller.Atualizar(id, questaoDto, _cancellationToken);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        Assert.NotNull(badRequestResult.Value);
        var mensagemProperty = badRequestResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro de validação", mensagemProperty?.GetValue(badRequestResult.Value));

        var errosProperty = badRequestResult.Value.GetType().GetProperty("erros");
        Assert.NotNull(errosProperty?.GetValue(badRequestResult.Value));
    }

    [Fact]
    public async Task Atualizar_NegocioException_DeveRetornarStatusDaExcecao()
    {
        const int id = 1;
        var questaoDto = new QuestaoDto { Id = id, Nome = "Questao" };
        var negocioException = new RegraNegocioException("Questao não encontrado", StatusCodes.Status404NotFound);

        _atualizarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, questaoDto, _cancellationToken))
            .ThrowsAsync(negocioException);

        var result = await _controller.Atualizar(id, questaoDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Questao não encontrado", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Atualizar_Exception_DeveRetornarStatus500()
    {
        const int id = 1;
        var questaoDto = new QuestaoDto { Id = id, Nome = "Questao" };
        var exception = new Exception("Erro interno");

        _atualizarQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, questaoDto, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Atualizar(id, questaoDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao atualizar questão", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogErrorWithId(_loggerMock, "Erro ao atualizar questão", exception, id);
    }

    #endregion

    #region Excluir Tests

    [Fact]
    public async Task Excluir_DeveRetornarNoContent_QuandoExclusaoComSucesso()
    {
        const int id = 1;
        _excluirQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _controller.Excluir(id, _cancellationToken);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task Excluir_QuestaoNaoEncontrado_DeveRetornarNotFound()
    {
        const int id = 999;
        _excluirQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var result = await _controller.Excluir(id, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Questão com ID {id} não encontrada", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task Excluir_OperationCanceledException_DeveRetornarStatus499()
    {
        const int id = 1;
        _excluirQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Excluir(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogInformationWithId(_loggerMock, "Requisição de exclusão foi cancelada para ID", id);
    }

    [Fact]
    public async Task Excluir_Exception_DeveRetornarStatus500()
    {
        const int id = 1;
        var exception = new Exception("Erro interno");
        _excluirQuestaoUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Excluir(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao excluir questão", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogErrorWithId(_loggerMock, "Erro ao excluir questão", exception, id);
    }

    #endregion

    #region Helper Methods

    private static void VerifyLogInformation(Mock<ILogger<QuestaoController>> loggerMock, string message)
    {
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static void VerifyLogError(Mock<ILogger<QuestaoController>> loggerMock, string message, Exception exception)
    {
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static void VerifyLogInformationWithId(Mock<ILogger<QuestaoController>> loggerMock, string message, int id)
    {
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains(message) &&
                    v.ToString()!.Contains(id.ToString())),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static void VerifyLogErrorWithId(Mock<ILogger<QuestaoController>> loggerMock, string message, Exception exception, int id)
    {
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains(message) &&
                    v.ToString()!.Contains(id.ToString())),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion
}