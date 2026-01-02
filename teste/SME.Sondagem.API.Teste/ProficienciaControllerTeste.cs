using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using Xunit;

namespace SME.Sondagem.API.Teste;

public class ProficienciaControllerTeste
{
    private readonly Mock<IObterProficienciasUseCase> _obterProficienciasUseCaseMock;
    private readonly Mock<IObterProficienciaPorIdUseCase> _obterProficienciaPorIdUseCaseMock;
    private readonly Mock<ICriarProficienciaUseCase> _criarProficienciaUseCaseMock;
    private readonly Mock<IAtualizarProficienciaUseCase> _atualizarProficienciaUseCaseMock;
    private readonly Mock<IExcluirProficienciaUseCase> _excluirProficienciaUseCaseMock;
    private readonly Mock<ILogger<ComponenteCurricularController>> _loggerMock;
    private readonly ProficienciaController _controller;
    private readonly CancellationToken _cancellationToken;

    public ProficienciaControllerTeste()
    {
        _obterProficienciasUseCaseMock = new Mock<IObterProficienciasUseCase>();
        _obterProficienciaPorIdUseCaseMock = new Mock<IObterProficienciaPorIdUseCase>();
        _criarProficienciaUseCaseMock = new Mock<ICriarProficienciaUseCase>();
        _atualizarProficienciaUseCaseMock = new Mock<IAtualizarProficienciaUseCase>();
        _excluirProficienciaUseCaseMock = new Mock<IExcluirProficienciaUseCase>();
        _loggerMock = new Mock<ILogger<ComponenteCurricularController>>();
        _cancellationToken = CancellationToken.None;

        _controller = new ProficienciaController(
            _obterProficienciasUseCaseMock.Object,
            _obterProficienciaPorIdUseCaseMock.Object,
            _criarProficienciaUseCaseMock.Object,
            _atualizarProficienciaUseCaseMock.Object,
            _excluirProficienciaUseCaseMock.Object,
            _loggerMock.Object
        );
    }

    #region Get Tests

    [Fact]
    public async Task Get_DeveRetornarOk_ComListaDeProficiencias()
    {
        var proficiencias = new List<ProficienciaDto>
        {
            new ProficienciaDto { Id = 1, Nome = "Proficiência 1" },
            new ProficienciaDto { Id = 2, Nome = "Proficiência 2" }
        };

        _obterProficienciasUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ReturnsAsync(proficiencias);

        var result = await _controller.Get(_cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(proficiencias, okResult.Value);
    }

    [Fact]
    public async Task Get_OperationCanceledException_DeveRetornarStatus499()
    {
        _obterProficienciasUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Get(_cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Information, "Requisição de listagem foi cancelada");
    }

    [Fact]
    public async Task Get_Exception_DeveRetornarStatus500()
    {
        var exception = new Exception("Erro interno");
        _obterProficienciasUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Get(_cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao listar proficiências", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Error, "Erro ao listar proficiências", exception);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_DeveRetornarOk_ComProficiencia()
    {
        const long id = 1;
        var proficiencia = new ProficienciaDto { Id = 1, Nome = "Proficiência Teste" };

        _obterProficienciaPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(proficiencia);

        var result = await _controller.GetById(id, _cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(proficiencia, okResult.Value);
    }

    [Fact]
    public async Task GetById_ProficienciaNaoEncontrada_DeveRetornarNotFound()
    {
        const long id = 999;
        _obterProficienciaPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync((ProficienciaDto?)null);

        var result = await _controller.GetById(id, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Proficiência com ID {id} não encontrada", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task GetById_OperationCanceledException_DeveRetornarStatus499()
    {
        const long id = 1;
        _obterProficienciaPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.GetById(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Information, "Requisição de obtenção foi cancelada para ID", additionalInfo: id.ToString());
    }

    [Fact]
    public async Task GetById_Exception_DeveRetornarStatus500()
    {
        const long id = 1;
        var exception = new Exception("Erro interno");
        _obterProficienciaPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.GetById(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao obter proficiência", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Error, "Erro ao obter proficiência", exception, id.ToString());
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_DeveRetornarCreated_ComProficienciaCriada()
    {
        var proficienciaDto = new ProficienciaDto { Nome = "Nova Proficiência" };
        const long proficienciaId = 1;

        _criarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(proficienciaDto, _cancellationToken))
            .ReturnsAsync(proficienciaId);

        var result = await _controller.Create(proficienciaDto, _cancellationToken);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(nameof(ProficienciaController.GetById), createdResult.ActionName);
        Assert.Equal(proficienciaId, createdResult.Value);

        var routeValues = createdResult.RouteValues;
        Assert.NotNull(routeValues);
        Assert.Equal(proficienciaId, routeValues["id"]);
    }

    [Fact]
    public async Task Create_OperationCanceledException_DeveRetornarStatus499()
    {
        var proficienciaDto = new ProficienciaDto { Nome = "Nova Proficiência" };
        _criarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(proficienciaDto, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Create(proficienciaDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Information, "Requisição de criação foi cancelada");
    }

    [Fact]
    public async Task Create_ValidationException_DeveRetornarBadRequest()
    {
        var proficienciaDto = new ProficienciaDto { Nome = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        };
        var validationException = new ValidationException(validationFailures);

        _criarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(proficienciaDto, _cancellationToken))
            .ThrowsAsync(validationException);

        var result = await _controller.Create(proficienciaDto, _cancellationToken);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        Assert.NotNull(badRequestResult.Value);
        var mensagemProperty = badRequestResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro de validação", mensagemProperty?.GetValue(badRequestResult.Value));

        var errosProperty = badRequestResult.Value.GetType().GetProperty("erros");
        Assert.NotNull(errosProperty?.GetValue(badRequestResult.Value));
    }

    [Fact]
    public async Task Create_NegocioException_DeveRetornarStatusDaExcecao()
    {
        var proficienciaDto = new ProficienciaDto { Nome = "Proficiência Duplicada" };
        var negocioException = new RegraNegocioException("Proficiência já existe", StatusCodes.Status409Conflict);

        _criarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(proficienciaDto, _cancellationToken))
            .ThrowsAsync(negocioException);

        var result = await _controller.Create(proficienciaDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Proficiência já existe", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Create_Exception_DeveRetornarStatus500()
    {
        var proficienciaDto = new ProficienciaDto { Nome = "Nova Proficiência" };
        var exception = new Exception("Erro interno");

        _criarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(proficienciaDto, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Create(proficienciaDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao criar proficiência", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Error, "Erro ao criar proficiência", exception);
    }

    #endregion

    #region Atualizar Tests

    [Fact]
    public async Task Atualizar_DeveRetornarOk_ComProficienciaAtualizada()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto { Id = id, Nome = "Proficiência Atualizada" };

        _atualizarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, proficienciaDto, _cancellationToken))
            .ReturnsAsync(proficienciaDto);

        var result = await _controller.Atualizar(id, proficienciaDto, _cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(proficienciaDto, okResult.Value);
    }

    [Fact]
    public async Task Atualizar_ProficienciaNaoEncontrada_DeveRetornarNotFound()
    {
        const int id = 999;
        var proficienciaDto = new ProficienciaDto { Id = id, Nome = "Proficiência" };

        _atualizarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, proficienciaDto, _cancellationToken))
            .ReturnsAsync((ProficienciaDto?)null);

        var result = await _controller.Atualizar(id, proficienciaDto, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Proficiência com ID {id} não encontrada", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task Atualizar_OperationCanceledException_DeveRetornarStatus499()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto { Id = id, Nome = "Proficiência Atualizada" };

        _atualizarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, proficienciaDto, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Atualizar(id, proficienciaDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Information, "Requisição de atualização foi cancelada para ID", additionalInfo: id.ToString());
    }

    [Fact]
    public async Task Atualizar_ValidationException_DeveRetornarBadRequest()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto { Id = id, Nome = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        };
        var validationException = new ValidationException(validationFailures);

        _atualizarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, proficienciaDto, _cancellationToken))
            .ThrowsAsync(validationException);

        var result = await _controller.Atualizar(id, proficienciaDto, _cancellationToken);

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
        var proficienciaDto = new ProficienciaDto { Id = id, Nome = "Proficiência" };
        var negocioException = new RegraNegocioException("Proficiência não encontrada", StatusCodes.Status404NotFound);

        _atualizarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, proficienciaDto, _cancellationToken))
            .ThrowsAsync(negocioException);

        var result = await _controller.Atualizar(id, proficienciaDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Proficiência não encontrada", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Atualizar_Exception_DeveRetornarStatus500()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto { Id = id, Nome = "Proficiência" };
        var exception = new Exception("Erro interno");

        _atualizarProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, proficienciaDto, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Atualizar(id, proficienciaDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao atualizar proficiência", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Error, "Erro ao atualizar proficiência", exception, id.ToString());
    }

    #endregion

    #region Excluir Tests

    [Fact]
    public async Task Excluir_DeveRetornarNoContent_QuandoExclusaoComSucesso()
    {
        const int id = 1;
        _excluirProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _controller.Excluir(id, _cancellationToken);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task Excluir_ProficienciaNaoEncontrada_DeveRetornarNotFound()
    {
        const int id = 999;
        _excluirProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var result = await _controller.Excluir(id, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Proficiência com ID {id} não encontrada", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task Excluir_OperationCanceledException_DeveRetornarStatus499()
    {
        const int id = 1;
        _excluirProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Excluir(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Information, "Requisição de exclusão foi cancelada para ID", additionalInfo: id.ToString());
    }

    [Fact]
    public async Task Excluir_Exception_DeveRetornarStatus500()
    {
        const int id = 1;
        var exception = new Exception("Erro interno");
        _excluirProficienciaUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Excluir(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao excluir proficiência", mensagemProperty?.GetValue(statusCodeResult.Value));

        _loggerMock.VerifyLog(LogLevel.Error, "Erro ao excluir proficiência", exception, id.ToString());
    }

    #endregion
}

public static class LoggerMockExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel logLevel, string message, Exception? exception = null, string? additionalInfo = null)
    {
        loggerMock.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains(message) &&
                    (additionalInfo == null || v.ToString()!.Contains(additionalInfo))),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}