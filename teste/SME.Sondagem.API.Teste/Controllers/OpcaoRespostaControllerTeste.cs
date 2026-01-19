using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller
{
    public class OpcaoRespostaControllerTeste
    {
        private readonly Mock<ICriarOpcaoRespostaUseCase> _mockCriarUseCase;
        private readonly Mock<IAtualizarOpcaoRespostaUseCase> _mockAtualizarUseCase;
        private readonly Mock<IExcluirOpcaoRespostaUseCase> _mockExcluirUseCase;
        private readonly Mock<IObterOpcaoRespostaPorIdUseCase> _mockObterPorIdUseCase;
        private readonly Mock<IObterOpcaoRespostaUseCase> _mockObterUseCase;
        private readonly OpcaoRespostaController _controller;
        private readonly CancellationToken _cancellationToken;

        public OpcaoRespostaControllerTeste()
        {
            _mockCriarUseCase = new Mock<ICriarOpcaoRespostaUseCase>();
            _mockAtualizarUseCase = new Mock<IAtualizarOpcaoRespostaUseCase>();
            _mockExcluirUseCase = new Mock<IExcluirOpcaoRespostaUseCase>();
            _mockObterPorIdUseCase = new Mock<IObterOpcaoRespostaPorIdUseCase>();
            _mockObterUseCase = new Mock<IObterOpcaoRespostaUseCase>();

            _controller = new OpcaoRespostaController(
                _mockCriarUseCase.Object,
                _mockAtualizarUseCase.Object,
                _mockExcluirUseCase.Object,
                _mockObterPorIdUseCase.Object,
                _mockObterUseCase.Object
            );

            _cancellationToken = CancellationToken.None;
        }

        #region Listar Tests

        [Fact]
        public async Task Listar_DeveRetornarOk_QuandoObterOpcaoRespostasComSucesso()
        {
            var opcaoRespostas = new List<OpcaoRespostaDto>
            {
                new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção 1" },
                new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção 2" }
            };
            _mockObterUseCase.Setup(x => x.ExecutarAsync(_cancellationToken))
                .ReturnsAsync(opcaoRespostas);

            var result = await _controller.Listar(_cancellationToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<IEnumerable<OpcaoRespostaDto>>(okResult.Value, exactMatch: false);
            Assert.Equal(2, returnValue.Count());
            _mockObterUseCase.Verify(x => x.ExecutarAsync(_cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Listar_DeveRetornarStatus499_QuandoOperacaoCancelada()
        {
            _mockObterUseCase.Setup(x => x.ExecutarAsync(_cancellationToken))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.Listar(_cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Listar_DeveRetornarStatus500_QuandoOcorreExcecao()
        {
            _mockObterUseCase.Setup(x => x.ExecutarAsync(_cancellationToken))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controller.Listar(_cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region ObterPorId Tests

        [Fact]
        public async Task ObterPorId_DeveRetornarOk_QuandoOpcaoRespostaEncontrada()
        {
            var id = 1L;
            var opcaoResposta = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção 1" };
            _mockObterPorIdUseCase.Setup(x => x.ExecutarAsync(id, _cancellationToken))
                .ReturnsAsync(opcaoResposta);

            var result = await _controller.ObterPorId(id, _cancellationToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OpcaoRespostaDto>(okResult.Value);
            Assert.Equal("Opção 1", returnValue.DescricaoOpcaoResposta);
            _mockObterPorIdUseCase.Verify(x => x.ExecutarAsync(id, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNotFound_QuandoOpcaoRespostaNaoEncontrada()
        {
            var id = 1L;
            _mockObterPorIdUseCase.Setup(x => x.ExecutarAsync(id, _cancellationToken))
                .ReturnsAsync((OpcaoRespostaDto?)null);

            var result = await _controller.ObterPorId(id, _cancellationToken);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarStatus499_QuandoOperacaoCancelada()
        {
            var id = 1L;
            _mockObterPorIdUseCase.Setup(x => x.ExecutarAsync(id, _cancellationToken))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.ObterPorId(id, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarStatus500_QuandoOcorreExcecao()
        {
            var id = 1L;
            _mockObterPorIdUseCase.Setup(x => x.ExecutarAsync(id, _cancellationToken))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controller.ObterPorId(id, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region Criar Tests

        [Fact]
        public async Task Criar_DeveRetornarCreated_QuandoCriadoComSucesso()
        {
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Nova Opção" };
            var novoId = 1L;
            _mockCriarUseCase.Setup(x => x.ExecutarAsync(dto, _cancellationToken))
                .ReturnsAsync(novoId);

            var result = await _controller.Criar(dto, _cancellationToken);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(nameof(OpcaoRespostaController.ObterPorId), createdResult.ActionName);
            _mockCriarUseCase.Verify(x => x.ExecutarAsync(dto, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Criar_DeveRetornarBadRequest_QuandoOcorreErroDeValidacao()
        {
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "" };
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("DescricaoOpcaoResposta", "Descrição é obrigatória")
            };
            var validationException = new ValidationException(validationFailures);
            _mockCriarUseCase.Setup(x => x.ExecutarAsync(dto, _cancellationToken))
                .ThrowsAsync(validationException);

            var result = await _controller.Criar(dto, _cancellationToken);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Criar_DeveRetornarStatusCode_QuandoOcorreRegraNegocioException()
        {
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção Duplicada" };
            var regraNegocioException = new RegraNegocioException("Opção de resposta já existe", 409);
            _mockCriarUseCase.Setup(x => x.ExecutarAsync(dto, _cancellationToken))
                .ThrowsAsync(regraNegocioException);

            var result = await _controller.Criar(dto, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Criar_DeveRetornarStatus499_QuandoOperacaoCancelada()
        {
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Nova Opção" };
            _mockCriarUseCase.Setup(x => x.ExecutarAsync(dto, _cancellationToken))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.Criar(dto, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Criar_DeveRetornarStatus500_QuandoOcorreExcecao()
        {
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Nova Opção" };
            _mockCriarUseCase.Setup(x => x.ExecutarAsync(dto, _cancellationToken))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controller.Criar(dto, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region Atualizar Tests

        [Fact]
        public async Task Atualizar_DeveRetornarOk_QuandoAtualizadoComSucesso()
        {
            var id = 1L;
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção Atualizada" };
            _mockAtualizarUseCase.Setup(x => x.ExecutarAsync(id, dto, _cancellationToken))
                .ReturnsAsync(dto);

            var result = await _controller.Atualizar(id, dto, _cancellationToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OpcaoRespostaDto>(okResult.Value);
            Assert.Equal("Opção Atualizada", returnValue.DescricaoOpcaoResposta);
            _mockAtualizarUseCase.Verify(x => x.ExecutarAsync(id, dto, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarNotFound_QuandoOpcaoRespostaNaoEncontrada()
        {
            var id = 1L;
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção Atualizada" };
            _mockAtualizarUseCase.Setup(x => x.ExecutarAsync(id, dto, _cancellationToken))
                .ReturnsAsync((OpcaoRespostaDto?)null);

            var result = await _controller.Atualizar(id, dto, _cancellationToken);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarBadRequest_QuandoOcorreErroDeValidacao()
        {
            var id = 1L;
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "" };
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("DescricaoOpcaoResposta", "Descrição é obrigatória")
            };
            var validationException = new ValidationException(validationFailures);
            _mockAtualizarUseCase.Setup(x => x.ExecutarAsync(id, dto, _cancellationToken))
                .ThrowsAsync(validationException);

            var result = await _controller.Atualizar(id, dto, _cancellationToken);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarStatusCode_QuandoOcorreRegraNegocioException()
        {
            var id = 1L;
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção Duplicada" };
            var regraNegocioException = new RegraNegocioException("Opção de resposta já existe", 409);
            _mockAtualizarUseCase.Setup(x => x.ExecutarAsync(id, dto, _cancellationToken))
                .ThrowsAsync(regraNegocioException);

            var result = await _controller.Atualizar(id, dto, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarStatus499_QuandoOperacaoCancelada()
        {
            var id = 1L;
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção Atualizada" };
            _mockAtualizarUseCase.Setup(x => x.ExecutarAsync(id, dto, _cancellationToken))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.Atualizar(id, dto, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarStatus500_QuandoOcorreExcecao()
        {
            var id = 1L;
            var dto = new OpcaoRespostaDto { DescricaoOpcaoResposta = "Opção Atualizada" };
            _mockAtualizarUseCase.Setup(x => x.ExecutarAsync(id, dto, _cancellationToken))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controller.Atualizar(id, dto, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region Excluir Tests

        [Fact]
        public async Task Excluir_DeveRetornarNoContent_QuandoExcluidoComSucesso()
        {
            var id = 1L;
            _mockExcluirUseCase.Setup(x => x.ExecutarAsync(id, _cancellationToken))
                .ReturnsAsync(true);

            var result = await _controller.Excluir(id, _cancellationToken);

            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            _mockExcluirUseCase.Verify(x => x.ExecutarAsync(id, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Excluir_DeveRetornarNotFound_QuandoOpcaoRespostaNaoEncontrada()
        {
            var id = 1L;
            _mockExcluirUseCase.Setup(x => x.ExecutarAsync(id, _cancellationToken))
                .ReturnsAsync(false);

            var result = await _controller.Excluir(id, _cancellationToken);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Excluir_DeveRetornarStatus499_QuandoOperacaoCancelada()
        {
            var id = 1L;
            _mockExcluirUseCase.Setup(x => x.ExecutarAsync(id, _cancellationToken))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.Excluir(id, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Excluir_DeveRetornarStatus500_QuandoOcorreExcecao()
        {
            var id = 1L;
            _mockExcluirUseCase.Setup(x => x.ExecutarAsync(id, _cancellationToken))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controller.Excluir(id, _cancellationToken);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        #endregion
    }
}
