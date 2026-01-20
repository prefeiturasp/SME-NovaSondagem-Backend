using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste.Controllers
{
    public class QuestaoOpcaoRespostaIntegracaoControllerTeste
    {
        private readonly Mock<IObterQuestaoOpcaoRespostaUseCase> obterMock = new();
        private readonly Mock<IObterQuestaoOpcaoRespostaPorIdUseCase> obterPorIdMock = new();
        private readonly Mock<ICriarQuestaoOpcaoRespostaUseCase> criarMock = new();
        private readonly Mock<IAtualizarQuestaoOpcaoRespostaUseCase> atualizarMock = new();
        private readonly Mock<IExcluirQuestaoOpcaoRespostaUseCase> excluirMock = new();

        private QuestaoOpcaoRespostaIntegracaoController CriarController()
            => new(
                obterMock.Object,
                obterPorIdMock.Object,
                criarMock.Object,
                atualizarMock.Object,
                excluirMock.Object);

        #region GET

        [Fact]
        public async Task Get_DeveRetornarOkComResultado()
        {
            var lista = new List<QuestaoOpcaoRespostaDto> { new() };

            obterMock
                .Setup(s => s.ExecutarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(lista);

            var controller = CriarController();

            var result = await controller.Get(CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(lista, ok.Value);
        }

        #endregion

        #region GET BY ID

        [Fact]
        public async Task GetById_DeveRetornarOk_QuandoEncontrar()
        {
            var dto = new QuestaoOpcaoRespostaDto();

            obterPorIdMock
                .Setup(s => s.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var controller = CriarController();

            var result = await controller.GetById(1, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task GetById_DeveLancarRegraNegocioException_QuandoNaoEncontrar()
        {
            obterPorIdMock
                .Setup(s => s.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((QuestaoOpcaoRespostaDto?)null);

            var controller = CriarController();

            var ex = await Assert.ThrowsAsync<RegraNegocioException>(
                () => controller.GetById(1, CancellationToken.None));

            Assert.Equal(404, ex.StatusCode);
        }

        #endregion

        #region CREATE

        [Fact]
        public async Task Create_DeveRetornarCreatedAtAction()
        {
            var dto = new QuestaoOpcaoRespostaDto();
            criarMock
                .Setup(s => s.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(10L);

            var controller = CriarController();

            var result = await controller.Create(dto, CancellationToken.None);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(QuestaoOpcaoRespostaIntegracaoController.GetById), created.ActionName);
            Assert.Equal(10L, created.Value);
        }

        #endregion

        #region ATUALIZAR

        [Fact]
        public async Task Atualizar_DeveRetornarOk_QuandoAtualizar()
        {
            var dto = new QuestaoOpcaoRespostaDto();

            atualizarMock
                .Setup(s => s.ExecutarAsync(1, dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var controller = CriarController();

            var result = await controller.Atualizar(1, dto, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task Atualizar_DeveLancarRegraNegocioException_QuandoNaoEncontrar()
        {
            atualizarMock
                .Setup(s => s.ExecutarAsync(1, It.IsAny<QuestaoOpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((QuestaoOpcaoRespostaDto?)null);

            var controller = CriarController();

            var ex = await Assert.ThrowsAsync<RegraNegocioException>(
                () => controller.Atualizar(1, new QuestaoOpcaoRespostaDto(), CancellationToken.None));

            Assert.Equal(404, ex.StatusCode);
        }

        #endregion

        #region EXCLUIR

        [Fact]
        public async Task Excluir_DeveRetornarNoContent_QuandoSucesso()
        {
            excluirMock
                .Setup(s => s.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var controller = CriarController();

            var result = await controller.Excluir(1, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Excluir_DeveLancarRegraNegocioException_QuandoNaoEncontrar()
        {
            excluirMock
                .Setup(s => s.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var controller = CriarController();

            var ex = await Assert.ThrowsAsync<RegraNegocioException>(
                () => controller.Excluir(1, CancellationToken.None));

            Assert.Equal(404, ex.StatusCode);
        }

        #endregion
    }
}
