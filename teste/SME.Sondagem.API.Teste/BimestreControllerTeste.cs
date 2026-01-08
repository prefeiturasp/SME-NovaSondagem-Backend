using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;
using Xunit;

namespace SME.Sondagem.API.Teste;

public class BimestreControllerTeste
{
    private readonly Mock<IObterBimestresUseCase> _obterBimestresUseCaseMock;
    private readonly Mock<IObterBimestrePorIdUseCase> _obterBimestrePorIdUseCaseMock;
    private readonly Mock<ICriarBimestreUseCase> _criarBimestreUseCaseMock;
    private readonly Mock<IAtualizarBimestreUseCase> _atualizarBimestreUseCaseMock;
    private readonly Mock<IExcluirBimestreUseCase> _excluirBimestreUseCaseMock;
    private readonly BimestreController _controller;
    private readonly CancellationToken _cancellationToken;

    public BimestreControllerTeste()
    {
        _obterBimestresUseCaseMock = new Mock<IObterBimestresUseCase>();
        _obterBimestrePorIdUseCaseMock = new Mock<IObterBimestrePorIdUseCase>();
        _criarBimestreUseCaseMock = new Mock<ICriarBimestreUseCase>();
        _atualizarBimestreUseCaseMock = new Mock<IAtualizarBimestreUseCase>();
        _excluirBimestreUseCaseMock = new Mock<IExcluirBimestreUseCase>();
        _cancellationToken = CancellationToken.None;


        _controller = new BimestreController(
            _criarBimestreUseCaseMock.Object,
            _atualizarBimestreUseCaseMock.Object,
            _excluirBimestreUseCaseMock.Object,
            _obterBimestrePorIdUseCaseMock.Object,
            _obterBimestresUseCaseMock.Object
        );
    }

    #region Listar Tests

    [Fact]
    public async Task Listar_DeveRetornarOk_ComListaDeBimestres()
    {
        var bimestres = new List<BimestreDto>
        {
            new BimestreDto { Id = 1, Descricao = "Bimestre 1" },
            new BimestreDto { Id = 2, Descricao = "Bimestre 2" }
        };

        _obterBimestresUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ReturnsAsync(bimestres);

        var result = await _controller.Listar(_cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(bimestres, okResult.Value);
    }

    [Fact]
    public async Task Listar_OperationCanceledException_DeveRetornarStatus499()
    {
        _obterBimestresUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Listar(_cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Listar_Exception_DeveRetornarStatus500()
    {
        var exception = new Exception("Erro interno");
        _obterBimestresUseCaseMock
            .Setup(x => x.ExecutarAsync(_cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Listar(_cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao listar bimestres", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    #endregion

    #region ObterPorId Tests

    [Fact]
    public async Task ObterPorId_DeveRetornarOk_ComBimestre()
    {
        const long id = 1;
        var bimestre = new BimestreDto { Id = 1, Descricao = "Bimestre Teste" };

        _obterBimestrePorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(bimestre);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(bimestre, okResult.Value);
    }

    [Fact]
    public async Task ObterPorId_BimestreNaoEncontrado_DeveRetornarNotFound()
    {
        const long id = 999;
        _obterBimestrePorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync((BimestreDto?)null);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Bimestre com ID {id} não encontrado", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task ObterPorId_OperationCanceledException_DeveRetornarStatus499()
    {
        const long id = 1;
        _obterBimestrePorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task ObterPorId_Exception_DeveRetornarStatus500()
    {
        const long id = 1;
        var exception = new Exception("Erro interno");
        _obterBimestrePorIdUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao obter bimestre", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    #endregion

    #region Criar Tests

    [Fact]
    public async Task Criar_DeveRetornarCreated_ComBimestreCriado()
    {
        var bimestreDto = new BimestreDto { Descricao = "Novo Bimestre" };
        const long bimestreId = 1;

        _criarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(bimestreDto, _cancellationToken))
            .ReturnsAsync(bimestreId);

        var result = await _controller.Criar(bimestreDto, _cancellationToken);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(nameof(BimestreController.ObterPorId), createdResult.ActionName);

        var routeValues = createdResult.RouteValues;
        Assert.NotNull(routeValues);
        Assert.Equal(bimestreId, routeValues["id"]);

        var resultValue = createdResult.Value;
        Assert.NotNull(resultValue);
        var idProperty = resultValue.GetType().GetProperty("id");
        Assert.Equal(bimestreId, idProperty?.GetValue(resultValue));
    }

    [Fact]
    public async Task Criar_OperationCanceledException_DeveRetornarStatus499()
    {
        var bimestreDto = new BimestreDto { Descricao = "Novo Bimestre" };
        _criarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(bimestreDto, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Criar(bimestreDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Criar_ValidationException_DeveRetornarBadRequest()
    {
        var bimestreDto = new BimestreDto { Descricao = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        };
        var validationException = new ValidationException(validationFailures);

        _criarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(bimestreDto, _cancellationToken))
            .ThrowsAsync(validationException);

        var result = await _controller.Criar(bimestreDto, _cancellationToken);

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
        var bimestreDto = new BimestreDto { Descricao = "Bimestre Duplicado" };
        var negocioException = new RegraNegocioException("Bimestre já existe", StatusCodes.Status409Conflict);

        _criarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(bimestreDto, _cancellationToken))
            .ThrowsAsync(negocioException);

        var result = await _controller.Criar(bimestreDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Bimestre já existe", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Criar_Exception_DeveRetornarStatus500()
    {
        var bimestreDto = new BimestreDto { Descricao = "Novo Bimestre" };
        var exception = new Exception("Erro interno");

        _criarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(bimestreDto, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Criar(bimestreDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao criar bimestre", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    #endregion

    #region Atualizar Tests

    [Fact]
    public async Task Atualizar_DeveRetornarOk_ComBimestreAtualizado()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto { Id = id, Descricao = "Bimestre Atualizado" };

        _atualizarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, bimestreDto, _cancellationToken))
            .ReturnsAsync(bimestreDto);

        var result = await _controller.Atualizar(id, bimestreDto, _cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(bimestreDto, okResult.Value);
    }

    [Fact]
    public async Task Atualizar_BimestreNaoEncontrado_DeveRetornarNotFound()
    {
        const int id = 999;
        var bimestreDto = new BimestreDto { Id = id, Descricao = "Bimestre" };

        _atualizarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, bimestreDto, _cancellationToken))
            .ReturnsAsync((BimestreDto?)null);

        var result = await _controller.Atualizar(id, bimestreDto, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Bimestre com ID {id} não encontrado", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task Atualizar_OperationCanceledException_DeveRetornarStatus499()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto { Id = id, Descricao = "Bimestre Atualizado" };

        _atualizarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, bimestreDto, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Atualizar(id, bimestreDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Atualizar_ValidationException_DeveRetornarBadRequest()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto { Id = id, Descricao = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        };
        var validationException = new ValidationException(validationFailures);

        _atualizarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, bimestreDto, _cancellationToken))
            .ThrowsAsync(validationException);

        var result = await _controller.Atualizar(id, bimestreDto, _cancellationToken);

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
        var bimestreDto = new BimestreDto { Id = id, Descricao = "Bimestre" };
        var negocioException = new RegraNegocioException("Bimestre não encontrado", StatusCodes.Status404NotFound);

        _atualizarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, bimestreDto, _cancellationToken))
            .ThrowsAsync(negocioException);

        var result = await _controller.Atualizar(id, bimestreDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Bimestre não encontrado", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Atualizar_Exception_DeveRetornarStatus500()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto { Id = id, Descricao = "Bimestre" };
        var exception = new Exception("Erro interno");

        _atualizarBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, bimestreDto, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Atualizar(id, bimestreDto, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao atualizar bimestre", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    #endregion

    #region Excluir Tests

    [Fact]
    public async Task Excluir_DeveRetornarNoContent_QuandoExclusaoComSucesso()
    {
        const long id = 1;
        _excluirBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _controller.Excluir(id, _cancellationToken);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task Excluir_BimestreNaoEncontrado_DeveRetornarNotFound()
    {
        const long id = 999;
        _excluirBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var result = await _controller.Excluir(id, _cancellationToken);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        Assert.NotNull(notFoundResult.Value);
        var mensagemProperty = notFoundResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal($"Bimestre com ID {id} não encontrado", mensagemProperty?.GetValue(notFoundResult.Value));
    }

    [Fact]
    public async Task Excluir_OperationCanceledException_DeveRetornarStatus499()
    {
        const long id = 1;
        _excluirBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Excluir(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Requisição cancelada pelo cliente", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    [Fact]
    public async Task Excluir_Exception_DeveRetornarStatus500()
    {
        const long id = 1;
        var exception = new Exception("Erro interno");
        _excluirBimestreUseCaseMock
            .Setup(x => x.ExecutarAsync(id, _cancellationToken))
            .ThrowsAsync(exception);

        var result = await _controller.Excluir(id, _cancellationToken);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.NotNull(statusCodeResult.Value);
        var mensagemProperty = statusCodeResult.Value.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao excluir bimestre", mensagemProperty?.GetValue(statusCodeResult.Value));
    }

    #endregion
}