using Moq;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Sondagem
{
    public class ExcluirSondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioSondagem> repositorioMock;
        private readonly ExcluirSondagemUseCase useCase;

        public ExcluirSondagemUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioSondagem>();
            useCase = new ExcluirSondagemUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarFalse_QuandoSondagemNaoExistir()
        {
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem?)null);

            var resultado = await useCase.ExecutarAsync(1);

            Assert.False(resultado);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(
                r => r.RemoverLogico(It.IsAny<long>(), null,It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarTrue_QuandoExcluirComSucesso()
        {
            var sondagem = CriarSondagem();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(sondagem.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sondagem);

            repositorioMock
                .Setup(r => r.RemoverLogico(sondagem.Id, null,It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var resultado = await useCase.ExecutarAsync(sondagem.Id);

            Assert.True(resultado);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(sondagem.Id, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(
                r => r.RemoverLogico(sondagem.Id, null,It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarFalse_QuandoExcluirFalhar()
        {
            var sondagem = CriarSondagem();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(sondagem.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sondagem);

            repositorioMock
                .Setup(r => r.RemoverLogico(sondagem.Id, null,It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var resultado = await useCase.ExecutarAsync(sondagem.Id);

            Assert.False(resultado);

            repositorioMock.Verify(
                r => r.RemoverLogico(sondagem.Id, null,It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem CriarSondagem()
        {
            return new SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem(
                descricao: "Sondagem teste",
                dataAplicacao: System.DateTime.Today
            );
        }
    }
}
