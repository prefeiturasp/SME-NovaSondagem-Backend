using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller
{
    public class QuestaoIntegracaoControllerTeste
    {
        private readonly Mock<IObterQuestoesUseCase> obterMock = new();
        private readonly Mock<IObterQuestaoPorIdUseCase> obterPorIdMock = new();
        private readonly Mock<ICriarQuestaoUseCase> criarMock = new();
        private readonly Mock<IAtualizarQuestaoUseCase> atualizarMock = new();
        private readonly Mock<IExcluirQuestaoUseCase> excluirMock = new();

        private QuestaoIntegracaoController CriarController()
            => new(
                obterMock.Object,
                obterPorIdMock.Object,
                criarMock.Object,
                atualizarMock.Object,
                excluirMock.Object
            );

        private static QuestaoDto CriarDto()
        {
            return new QuestaoDto
            {
                Nome = "Nome Teste",
                Tipo = Dominio.Enums.TipoQuestao.Periodo
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
                          .ReturnsAsync((QuestaoDto?)null);

            var controller = CriarController();

            var result = await controller.GetById(1, CancellationToken.None);

            (result as NotFoundObjectResult)!.StatusCode.Should().Be(404);
        }

        #endregion

        #region CREATE

        [Fact]
        public async Task Create_DeveRetornarCreated()
        {
            criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            var controller = CriarController();

            var result = await controller.Create(CriarDto(), CancellationToken.None);

            (result as CreatedAtActionResult)!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Create_Exception_DeveRetornar500()
        {
            criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("Erro de teste"));

            var controller = CriarController();

            var result = await controller.Create(CriarDto(), CancellationToken.None);

            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(500);
        }

        #endregion

        #region UPDATE

        [Fact]
        public async Task Atualizar_DeveRetornarOk()
        {
            atualizarMock.Setup(x => x.ExecutarAsync(1, It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(CriarDto());

            var controller = CriarController();

            var result = await controller.Atualizar(1, CriarDto(), CancellationToken.None);

            (result as OkObjectResult)!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Atualizar_NaoEncontrado()
        {
            atualizarMock.Setup(x => x.ExecutarAsync(1, It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync((QuestaoDto?)null);

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
