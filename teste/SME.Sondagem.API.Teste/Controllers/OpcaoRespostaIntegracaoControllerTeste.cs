using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller
{
    public class OpcaoRespostaIntegracaoControllerTeste
    {
        private readonly Mock<IObterOpcaoRespostaUseCase> obterMock = new();
        private readonly Mock<IObterOpcaoRespostaPorIdUseCase> obterPorIdMock = new();
        private readonly Mock<ICriarOpcaoRespostaUseCase> criarMock = new();
        private readonly Mock<IAtualizarOpcaoRespostaUseCase> atualizarMock = new();
        private readonly Mock<IExcluirOpcaoRespostaUseCase> excluirMock = new();

        private OpcaoRespostaIntegracaoController CriarController()
            => new(
                obterMock.Object,
                obterPorIdMock.Object,
                criarMock.Object,
                atualizarMock.Object,
                excluirMock.Object
            );

        private static OpcaoRespostaDto CriarDto()
        {
            return new OpcaoRespostaDto
            {
                Id = 1,
                Ordem = 1,
                DescricaoOpcaoResposta = "Opção A",
                Legenda = "Legenda teste",
                CorFundo = "#FFFFFF",
                CorTexto = "#000000"
            };
        }

        #region GET

        [Fact]
        public async Task Get_DeveRetornarOk()
        {
            obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new[] { CriarDto() });

            var controller = CriarController();

            var result = await controller.Get(CancellationToken.None);

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Get_OperationCanceledException()
        {
            obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new OperationCanceledException());

            var controller = CriarController();

            var result = await controller.Get(CancellationToken.None);

            var status = result as ObjectResult;
            status!.StatusCode.Should().Be(499);
        }

        [Fact]
        public async Task Get_Exception()
        {
            obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception());

            var controller = CriarController();

            var result = await controller.Get(CancellationToken.None);

            (result as ObjectResult)!.StatusCode.Should().Be(500);
        }

        #endregion

        #region GET BY ID

        [Fact]
        public async Task GetById_DeveRetornarOk()
        {
            obterPorIdMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(CriarDto());

            var controller = CriarController();

            var result = await controller.GetById(1, CancellationToken.None);

            (result as OkObjectResult)!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetById_NaoEncontrado()
        {
            obterPorIdMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((OpcaoRespostaDto?)null);

            var controller = CriarController();

            var result = await controller.GetById(1, CancellationToken.None);

            (result as NotFoundObjectResult)!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetById_Exception()
        {
            obterPorIdMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception());

            var controller = CriarController();

            var result = await controller.GetById(1, CancellationToken.None);

            (result as ObjectResult)!.StatusCode.Should().Be(500);
        }

        #endregion

        #region CREATE

        [Fact]
        public async Task Create_DeveRetornarCreated()
        {
            criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            var controller = CriarController();

            var result = await controller.Create(CriarDto(), CancellationToken.None);

            (result as CreatedAtActionResult)!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Create_ValidationException()
        {
            var validationFailures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("DescricaoOpcaoResposta", "Campo obrigatório")
            };

            criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new FluentValidation.ValidationException(validationFailures));

            var controller = CriarController();

            var result = await controller.Create(CriarDto(), CancellationToken.None);

            (result as BadRequestObjectResult)!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Create_RegraNegocioException()
        {
            criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new RegraNegocioException("erro", 409));

            var controller = CriarController();

            var result = await controller.Create(CriarDto(), CancellationToken.None);

            (result as ObjectResult)!.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task Create_Exception()
        {
            criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception());

            var controller = CriarController();

            var result = await controller.Create(CriarDto(), CancellationToken.None);

            (result as ObjectResult)!.StatusCode.Should().Be(500);
        }

        #endregion

        #region UPDATE

        [Fact]
        public async Task Atualizar_DeveRetornarOk()
        {
            atualizarMock.Setup(x => x.ExecutarAsync(1, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(CriarDto());

            var controller = CriarController();

            var result = await controller.Atualizar(1, CriarDto(), CancellationToken.None);

            (result as OkObjectResult)!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Atualizar_NaoEncontrado()
        {
            atualizarMock.Setup(x => x.ExecutarAsync(1, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync((OpcaoRespostaDto?)null);

            var controller = CriarController();

            var result = await controller.Atualizar(1, CriarDto(), CancellationToken.None);

            (result as NotFoundObjectResult)!.StatusCode.Should().Be(404);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task Excluir_DeveRetornarNoContent()
        {
            excluirMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

            var controller = CriarController();

            var result = await controller.Excluir(1, CancellationToken.None);

            (result as NoContentResult)!.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task Excluir_NaoEncontrado()
        {
            excluirMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

            var controller = CriarController();

            var result = await controller.Excluir(1, CancellationToken.None);

            (result as NotFoundObjectResult)!.StatusCode.Should().Be(404);
        }
        #endregion
    }
}
