using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Turma;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste.Controllers
{
    public class TurmaControllerTeste
    {
        private readonly Mock<IObterPermissaoTurmaUseCase> obterPermissaoTurmaUseCaseMock;

        public TurmaControllerTeste()
        {
            obterPermissaoTurmaUseCaseMock = new Mock<IObterPermissaoTurmaUseCase>();
        }

        private static FiltroQuestionario CriarFiltro(int turmaId)
        {
            return new FiltroQuestionario() 
            { 
                TurmaId = turmaId
            };
        }

        [Fact]
        public async Task ValidarSondagemTurma_QuandoPermissaoValida_DeveRetornarOkComTrue()
        {
            // Arrange
            var filtro = CriarFiltro(123456);

            obterPermissaoTurmaUseCaseMock
                .Setup(x => x.ObterPermissaoTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var controller = new TurmaController();

            // Act
            var result = await controller.ObterPermissaoTurma(
                filtro.TurmaId,
                obterPermissaoTurmaUseCaseMock.Object,
                CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.True((bool)okResult.Value!);
        }

        [Fact]
        public async Task ValidarSondagemTurma_QuandoUseCaseLancaExcecao_DevePropagarExcecao()
        {
            // Arrange
            var filtro = CriarFiltro(123456);

            obterPermissaoTurmaUseCaseMock
                .Setup(x => x.ObterPermissaoTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RegraNegocioException("Erro de regra", 601));

            var controller = new TurmaController();

            // Act & Assert
            await Assert.ThrowsAsync<RegraNegocioException>(() =>
                controller.ObterPermissaoTurma(
                    filtro.TurmaId,
                    obterPermissaoTurmaUseCaseMock.Object,
                    CancellationToken.None));
        }
    }
}
