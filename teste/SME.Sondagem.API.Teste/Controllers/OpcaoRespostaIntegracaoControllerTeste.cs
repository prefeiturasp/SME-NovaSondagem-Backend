using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
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
            => new()
            {
                Id = 1,
                Ordem = 1,
                DescricaoOpcaoResposta = "Opção A",
                Legenda = "Legenda teste",
                CorFundo = "#FFFFFF",
                CorTexto = "#000000"
            };

        [Fact]
        public async Task Get_DeveRetornarOk()
        {
            obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new[] { CriarDto() });

            var controller = CriarController();

            var result = await controller.Get(CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetById_DeveRetornarOk()
        {
            obterPorIdMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(CriarDto());

            var controller = CriarController();

            var result = await controller.GetById(1, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetById_DeveLancarRegraNegocioException_QuandoNaoEncontrado()
        {
            obterPorIdMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((OpcaoRespostaDto?)null);

            var controller = CriarController();

            Func<Task> act = async () =>
                await controller.GetById(1, CancellationToken.None);

            await act.Should().ThrowAsync<RegraNegocioException>();
        }

        [Fact]
        public async Task Create_DeveRetornarCreated()
        {
            criarMock.Setup(x => x.ExecutarAsync(It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            var controller = CriarController();

            var result = await controller.Create(CriarDto(), CancellationToken.None);

            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            created.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarOk()
        {
            atualizarMock.Setup(x => x.ExecutarAsync(1, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(CriarDto());

            var controller = CriarController();

            var result = await controller.Atualizar(1, CriarDto(), CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Atualizar_DeveLancarRegraNegocioException_QuandoNaoEncontrado()
        {
            atualizarMock.Setup(x => x.ExecutarAsync(1, It.IsAny<OpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync((OpcaoRespostaDto?)null);

            var controller = CriarController();

            Func<Task> act = async () =>
                await controller.Atualizar(1, CriarDto(), CancellationToken.None);

            await act.Should().ThrowAsync<RegraNegocioException>();
        }

        [Fact]
        public async Task Excluir_DeveRetornarNoContent()
        {
            excluirMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

            var controller = CriarController();

            var result = await controller.Excluir(1, CancellationToken.None);

            var noContent = result.Should().BeOfType<NoContentResult>().Subject;
            noContent.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task Excluir_DeveLancarRegraNegocioException_QuandoNaoEncontrado()
        {
            excluirMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

            var controller = CriarController();

            Func<Task> act = async () =>
                await controller.Excluir(1, CancellationToken.None);

            await act.Should().ThrowAsync<RegraNegocioException>();
        }
    }
}
