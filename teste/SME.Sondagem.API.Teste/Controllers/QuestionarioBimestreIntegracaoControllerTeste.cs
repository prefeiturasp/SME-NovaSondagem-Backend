using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SME.Sondagem.API.Controllers.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;
using System.Net;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller
{
    public class QuestionarioBimestreIntegracaoControllerTeste
    {
        private readonly Mock<IVincularBimestresUseCase> _vincularBimestresUseCaseMock;
        private readonly Mock<IObterQuestionariosBimestresUseCase> _obterTodosUseCaseMock;
        private readonly Mock<IObterBimestresPorQuestionarioUseCase> _obterPorQuestionarioUseCaseMock;
        private readonly Mock<IExcluirVinculosPorQuestionarioUseCase> _excluirPorQuestionarioUseCaseMock;
        private readonly Mock<ILogger<QuestionarioBimestreIntegracaoController>> _loggerMock;
        private readonly QuestionarioBimestreIntegracaoController _controller;

        public QuestionarioBimestreIntegracaoControllerTeste()
        {
            _vincularBimestresUseCaseMock = new Mock<IVincularBimestresUseCase>();
            _obterTodosUseCaseMock = new Mock<IObterQuestionariosBimestresUseCase>();
            _obterPorQuestionarioUseCaseMock = new Mock<IObterBimestresPorQuestionarioUseCase>();
            _excluirPorQuestionarioUseCaseMock = new Mock<IExcluirVinculosPorQuestionarioUseCase>();
            _loggerMock = new Mock<ILogger<QuestionarioBimestreIntegracaoController>>();

            _controller = new QuestionarioBimestreIntegracaoController(
                _vincularBimestresUseCaseMock.Object,
                _obterTodosUseCaseMock.Object,
                _obterPorQuestionarioUseCaseMock.Object,
                _excluirPorQuestionarioUseCaseMock.Object,
                _loggerMock.Object);
        }

        #region Listar Tests

        [Fact]
        public async Task Listar_Deve_Retornar_Ok_Com_Lista_De_Vinculos()
        {
            var vinculos = new List<QuestionarioBimestreDto>
            {
                new QuestionarioBimestreDto { Id = 1, QuestionarioId = 10, BimestreId = 1 },
                new QuestionarioBimestreDto { Id = 2, QuestionarioId = 10, BimestreId = 2 }
            };

            _obterTodosUseCaseMock
                .Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculos);

            var result = await _controller.Listar(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var retorno = Assert.IsType<IEnumerable<QuestionarioBimestreDto>>(okResult.Value, exactMatch: false);
            Assert.Equal(2, retorno.Count());
        }

        [Fact]
        public async Task Listar_Deve_Retornar_499_Quando_Operacao_Cancelada()
        {
            _obterTodosUseCaseMock
                .Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.Listar(CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task Listar_Deve_Retornar_500_Quando_Ocorrer_Erro()
        {
            _obterTodosUseCaseMock
                .Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controller.Listar(CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region ObterPorQuestionario Tests

        [Fact]
        public async Task ObterPorQuestionario_Deve_Retornar_Ok_Com_Vinculos()
        {
            var questionarioId = 10;
            var vinculos = new List<QuestionarioBimestreDto>
            {
                new QuestionarioBimestreDto { Id = 1, QuestionarioId = questionarioId, BimestreId = 1 }
            };

            _obterPorQuestionarioUseCaseMock
                .Setup(x => x.ExecutarAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculos);

            var result = await _controller.ObterPorQuestionario(questionarioId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var retorno = Assert.IsType<IEnumerable<QuestionarioBimestreDto>>(okResult.Value, exactMatch: false);
            Assert.Single(retorno);
        }

        [Fact]
        public async Task ObterPorQuestionario_Deve_Retornar_499_Quando_Operacao_Cancelada()
        {
            _obterPorQuestionarioUseCaseMock
                .Setup(x => x.ExecutarAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.ObterPorQuestionario(1, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task ObterPorQuestionario_Deve_Retornar_500_Quando_Ocorrer_Erro()
        {
            _obterPorQuestionarioUseCaseMock
                .Setup(x => x.ExecutarAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro"));

            var result = await _controller.ObterPorQuestionario(1, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region VincularMultiplos Tests

        [Fact]
        public async Task VincularMultiplos_Deve_Retornar_Ok_Quando_Sucesso()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 2 }
            };

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.VincularMultiplos(dto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task VincularMultiplos_Deve_Retornar_400_Quando_Validacao_Falhar()
        {
            var dto = new VincularBimestresDto();
            var validationException = new ValidationException("Erro de validação");

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(validationException);

            var result = await _controller.VincularMultiplos(dto, CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task VincularMultiplos_Deve_Retornar_404_Quando_Questionario_Nao_Existe()
        {
            var dto = new VincularBimestresDto { QuestionarioId = 999 };
            var exception = new RegraNegocioException("Questionário não encontrado", HttpStatusCode.NotFound);

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var result = await _controller.VincularMultiplos(dto, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task VincularMultiplos_Deve_Retornar_409_Quando_Vinculo_Ja_Existe()
        {
            var dto = new VincularBimestresDto();
            var exception = new RegraNegocioException("Vínculo já existe", HttpStatusCode.Conflict);

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var result = await _controller.VincularMultiplos(dto, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, statusResult.StatusCode);
        }

        [Fact]
        public async Task VincularMultiplos_Deve_Retornar_499_Quando_Operacao_Cancelada()
        {
            var dto = new VincularBimestresDto();

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.VincularMultiplos(dto, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task VincularMultiplos_Deve_Retornar_500_Quando_Ocorrer_Erro()
        {
            var dto = new VincularBimestresDto();

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro"));

            var result = await _controller.VincularMultiplos(dto, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region AtualizarVinculos Tests

        [Fact]
        public async Task AtualizarVinculos_Deve_Retornar_Ok_Quando_Sucesso()
        {
            var questionarioId = 10;
            var dto = new AtualizarVinculosBimestresDto
            {
                BimestreIds = new List<int> { 1, 2, 3 }
            };

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAtualizacaoAsync(It.IsAny<AtualizarVinculosBimestresDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.AtualizarVinculos(questionarioId, dto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task AtualizarVinculos_Deve_Definir_QuestionarioId_No_Dto()
        {
            var questionarioId = 10;
            var dto = new AtualizarVinculosBimestresDto
            {
                BimestreIds = new List<int> { 1 }
            };

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAtualizacaoAsync(It.IsAny<AtualizarVinculosBimestresDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _controller.AtualizarVinculos(questionarioId, dto, CancellationToken.None);

            _vincularBimestresUseCaseMock.Verify(x => x.ExecutarAtualizacaoAsync(
                It.Is<AtualizarVinculosBimestresDto>(d => d.QuestionarioId == questionarioId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarVinculos_Deve_Retornar_400_Quando_QuestionarioId_Rota_Diferente_Do_Body()
        {
            var questionarioId = 10;
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 20,
                BimestreIds = new List<int> { 1 }
            };

            var result = await _controller.AtualizarVinculos(questionarioId, dto, CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task AtualizarVinculos_Deve_Retornar_400_Quando_Validacao_Falhar()
        {
            var dto = new AtualizarVinculosBimestresDto();
            var validationException = new ValidationException("Erro");

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAtualizacaoAsync(It.IsAny<AtualizarVinculosBimestresDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(validationException);

            var result = await _controller.AtualizarVinculos(1, dto, CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task AtualizarVinculos_Deve_Retornar_404_Quando_Questionario_Nao_Existe()
        {
            var dto = new AtualizarVinculosBimestresDto();
            var exception = new RegraNegocioException("Não encontrado", HttpStatusCode.NotFound);

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAtualizacaoAsync(It.IsAny<AtualizarVinculosBimestresDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var result = await _controller.AtualizarVinculos(1, dto, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task AtualizarVinculos_Deve_Retornar_499_Quando_Operacao_Cancelada()
        {
            var dto = new AtualizarVinculosBimestresDto();

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAtualizacaoAsync(It.IsAny<AtualizarVinculosBimestresDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.AtualizarVinculos(1, dto, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task AtualizarVinculos_Deve_Retornar_500_Quando_Ocorrer_Erro()
        {
            var dto = new AtualizarVinculosBimestresDto();

            _vincularBimestresUseCaseMock
                .Setup(x => x.ExecutarAtualizacaoAsync(It.IsAny<AtualizarVinculosBimestresDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro"));

            var result = await _controller.AtualizarVinculos(1, dto, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region ExcluirVinculos Tests

        [Fact]
        public async Task ExcluirVinculos_Deve_Retornar_NoContent_Quando_Sucesso()
        {
            var questionarioId = 10;

            _excluirPorQuestionarioUseCaseMock
                .Setup(x => x.ExecutarAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.ExcluirVinculos(questionarioId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ExcluirVinculos_Deve_Retornar_404_Quando_Nao_Encontrado()
        {
            var questionarioId = 999;

            _excluirPorQuestionarioUseCaseMock
                .Setup(x => x.ExecutarAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.ExcluirVinculos(questionarioId, CancellationToken.None);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task ExcluirVinculos_Deve_Retornar_499_Quando_Operacao_Cancelada()
        {
            _excluirPorQuestionarioUseCaseMock
                .Setup(x => x.ExecutarAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var result = await _controller.ExcluirVinculos(1, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task ExcluirVinculos_Deve_Retornar_500_Quando_Ocorrer_Erro()
        {
            _excluirPorQuestionarioUseCaseMock
                .Setup(x => x.ExecutarAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro"));

            var result = await _controller.ExcluirVinculos(1, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion
    }
}