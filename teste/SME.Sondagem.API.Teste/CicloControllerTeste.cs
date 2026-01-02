using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;
using Xunit;

namespace SME.Sondagem.API.Teste;

public class CicloControllerTeste
{
    private readonly Mock<IObterCiclosUseCase> _obterCiclosUseCaseMock;
    private readonly Mock<IObterCicloPorIdUseCase> _obterCicloPorIdUseCaseMock;
    private readonly Mock<ICriarCicloUseCase> _criarCicloUseCaseMock;
    private readonly Mock<IAtualizarCicloUseCase> _atualizarCicloUseCaseMock;
    private readonly Mock<IExcluirCicloUseCase> _excluirCicloUseCaseMock;
    private readonly Mock<ILogger<CicloController>> _loggerMock;
    private readonly CicloController _controller;
    private readonly CancellationToken _cancellationToken;

    public CicloControllerTeste()
    {
        _obterCiclosUseCaseMock = new Mock<IObterCiclosUseCase>();
        _obterCicloPorIdUseCaseMock = new Mock<IObterCicloPorIdUseCase>();
        _criarCicloUseCaseMock = new Mock<ICriarCicloUseCase>();
        _atualizarCicloUseCaseMock = new Mock<IAtualizarCicloUseCase>();
        _excluirCicloUseCaseMock = new Mock<IExcluirCicloUseCase>();
        _loggerMock = new Mock<ILogger<CicloController>>();
        _cancellationToken = CancellationToken.None;

        _controller = new CicloController(
            _criarCicloUseCaseMock.Object,
            _atualizarCicloUseCaseMock.Object,
            _excluirCicloUseCaseMock.Object,
            _obterCicloPorIdUseCaseMock.Object,
            _obterCiclosUseCaseMock.Object,
            _loggerMock.Object
        );
    }

    #region Listar Tests

    [Fact]
    public async Task Listar_DeveRetornarOk_ComListaDeCiclos()
    {
        var ciclos = new List<CicloDto>
        {
            new CicloDto { Id = 1, DescCiclo = "Ciclo 1" },
            new CicloDto { Id = 2, DescCiclo = "Ciclo 2" }
        };

        _obterCiclosUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ReturnsAsync(ciclos);

        var result = await _controller.Listar(_cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(ciclos, okResult.Value);
    }

    [Fact]
    public async Task Listar_OperationCanceledException_DeveRetornarStatus499()
    {
        _obterCiclosUseCaseMock
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
        _obterCiclosUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Listar(_cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao listar ciclos", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogError(_loggerMock, "Erro ao listar ciclos", exception);
    }

    #endregion

    #region ObterPorId Tests

    [Fact]
    public async Task ObterPorId_DeveRetornarOk_ComCiclo()
    {
        const long id = 1;
        var ciclo = new CicloDto { Id = 1, DescCiclo = "Ciclo Teste" };

        _obterCicloPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(ciclo);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(ciclo, okResult.Value);
    }

    [Fact]
    public async Task ObterPorId_CicloNaoEncontrado_DeveRetornarNotFound()
    {
        const long id = 999;
        _obterCicloPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync((CicloDto?)null);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Ciclo com ID {id} não encontrado", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task ObterPorId_OperationCanceledException_DeveRetornarStatus499()
    {
        const long id = 1;
        _obterCicloPorIdUseCaseMock
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
        const long id = 1;
        var exception = new Exception("Erro interno");
        _obterCicloPorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao obter ciclo", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogErrorWithId(_loggerMock, "Erro ao obter ciclo", exception, id);
    }

    #endregion

    #region Criar Tests

    [Fact]
    public async Task Criar_DeveRetornarCreated_ComCicloCriado()
    {
        var cicloDto = new CicloDto { DescCiclo = "Novo Ciclo" };
        const long cicloId = 1;

        _criarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(cicloDto, _cancellationToken))
            .ReturnsAsync(cicloId);

        var result = await _controller.Criar(cicloDto, _cancellationToken);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(nameof(CicloController.ObterPorId), createdResult.ActionName);

        var routeValues = createdResult.RouteValues;
        Assert.NotNull(routeValues);
        Assert.Equal(cicloId, routeValues["id"]);

        var resultValue = createdResult.Value;
        Assert.NotNull(resultValue);
        var idProperty = resultValue.GetType().GetProperty("id");
        Assert.Equal(cicloId, idProperty?.GetValue(resultValue));
    }

    [Fact]
    public async Task Criar_OperationCanceledException_DeveRetornarStatus499()
    {
        var cicloDto = new CicloDto { DescCiclo = "Novo Ciclo" };
        _criarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(cicloDto, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Criar(cicloDto, _cancellationToken);

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
        var cicloDto = new CicloDto { DescCiclo = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        };
        var validationException = new ValidationException(validationFailures);

        _criarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(cicloDto, _cancellationToken))
            .ThrowsAsync(validationException);

        var result = await _controller.Criar(cicloDto, _cancellationToken);

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
        var cicloDto = new CicloDto { DescCiclo = "Ciclo Duplicado" };
        var negocioException = new RegraNegocioException("Ciclo já existe", StatusCodes.Status409Conflict);

        _criarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(cicloDto, _cancellationToken))
            .ThrowsAsync(negocioException);

        var result = await _controller.Criar(cicloDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Ciclo já existe", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Criar_Exception_DeveRetornarStatus500()
    {
        var cicloDto = new CicloDto { DescCiclo = "Novo Ciclo" };
        var exception = new Exception("Erro interno");

        _criarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(cicloDto, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Criar(cicloDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao criar ciclo", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogError(_loggerMock, "Erro ao criar ciclo", exception);
    }

    #endregion

    #region Atualizar Tests

    [Fact]
    public async Task Atualizar_DeveRetornarOk_ComCicloAtualizado()
    {
        const int id = 1;
        var cicloDto = new CicloDto { Id = id, DescCiclo = "Ciclo Atualizado" };

        _atualizarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, cicloDto, _cancellationToken))
            .ReturnsAsync(cicloDto);

        var result = await _controller.Atualizar(id, cicloDto, _cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(cicloDto, okResult.Value);
    }

    [Fact]
    public async Task Atualizar_CicloNaoEncontrado_DeveRetornarNotFound()
    {
        const int id = 999;
        var cicloDto = new CicloDto { Id = id, DescCiclo = "Ciclo" };

        _atualizarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, cicloDto, _cancellationToken))
            .ReturnsAsync((CicloDto?)null);

        var result = await _controller.Atualizar(id, cicloDto, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Ciclo com ID {id} não encontrado", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task Atualizar_OperationCanceledException_DeveRetornarStatus499()
    {
        const int id = 1;
        var cicloDto = new CicloDto { Id = id, DescCiclo = "Ciclo Atualizado" };

        _atualizarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, cicloDto, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Atualizar(id, cicloDto, _cancellationToken);

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
        var cicloDto = new CicloDto { Id = id, DescCiclo = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        };
        var validationException = new ValidationException(validationFailures);

        _atualizarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, cicloDto, _cancellationToken))
            .ThrowsAsync(validationException);

        var result = await _controller.Atualizar(id, cicloDto, _cancellationToken);

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
        var cicloDto = new CicloDto { Id = id, DescCiclo = "Ciclo" };
        var negocioException = new RegraNegocioException("Ciclo não encontrado", StatusCodes.Status404NotFound);

        _atualizarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, cicloDto, _cancellationToken))
            .ThrowsAsync(negocioException);

        var result = await _controller.Atualizar(id, cicloDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Ciclo não encontrado", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Atualizar_Exception_DeveRetornarStatus500()
    {
        const int id = 1;
        var cicloDto = new CicloDto { Id = id, DescCiclo = "Ciclo" };
        var exception = new Exception("Erro interno");

        _atualizarCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, cicloDto, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Atualizar(id, cicloDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao atualizar ciclo", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogErrorWithId(_loggerMock, "Erro ao atualizar ciclo", exception, id);
    }

    #endregion

    #region Excluir Tests

    [Fact]
    public async Task Excluir_DeveRetornarNoContent_QuandoExclusaoComSucesso()
    {
        const long id = 1;
        _excluirCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _controller.Excluir(id, _cancellationToken);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task Excluir_CicloNaoEncontrado_DeveRetornarNotFound()
    {
        const long id = 999;
        _excluirCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var result = await _controller.Excluir(id, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Ciclo com ID {id} não encontrado", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task Excluir_OperationCanceledException_DeveRetornarStatus499()
    {
        const long id = 1;
        _excluirCicloUseCaseMock
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
        const long id = 1;
        var exception = new Exception("Erro interno");
        _excluirCicloUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Excluir(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao excluir ciclo", mensagemProperty?.GetValue(statusCodeResult.Value));

        VerifyLogErrorWithId(_loggerMock, "Erro ao excluir ciclo", exception, id);
    }

    #endregion

    #region Helper Methods

    private static void VerifyLogInformation(Mock<ILogger<CicloController>> loggerMock, string message)
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

    private static void VerifyLogError(Mock<ILogger<CicloController>> loggerMock, string message, Exception exception)
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

    private static void VerifyLogInformationWithId(Mock<ILogger<CicloController>> loggerMock, string message, long id)
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

    private static void VerifyLogErrorWithId(Mock<ILogger<CicloController>> loggerMock, string message, Exception exception, long id)
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