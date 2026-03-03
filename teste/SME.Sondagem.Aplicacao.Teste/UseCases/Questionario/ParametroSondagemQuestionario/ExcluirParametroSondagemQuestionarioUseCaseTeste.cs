using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagemQuestionario
{
    public class ExcluirParametroSondagemQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagemQuestionario> repositorioMock;
        private readonly ExcluirParametroSondagemQuestionarioUseCase useCase;

        public ExcluirParametroSondagemQuestionarioUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagemQuestionario>();
            useCase = new ExcluirParametroSondagemQuestionarioUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_parametro_nao_for_encontrado()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.ParametroSondagemQuestionario)null!);

            // Act
            var resultado = await useCase.ExecutarAsync(1);

            // Assert
            Assert.False(resultado);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(r =>
                r.RemoverLogico(It.IsAny<long>(), It.IsAny<string?>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_remocao_logica_nao_afetar_registro()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dominio.Entidades.ParametroSondagemQuestionario());

            repositorioMock
                .Setup(r => r.RemoverLogico(It.IsAny<long>(), It.IsAny<string?>()))
                .ReturnsAsync(0);

            // Act
            var resultado = await useCase.ExecutarAsync(1);

            // Assert
            Assert.False(resultado);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(r =>
                r.RemoverLogico(1, It.IsAny<string?>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_true_quando_remocao_logica_ocorrer_com_sucesso()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dominio.Entidades.ParametroSondagemQuestionario());

            repositorioMock
                .Setup(r => r.RemoverLogico(It.IsAny<long>(), It.IsAny<string?>()))
                .ReturnsAsync(1);

            // Act
            var resultado = await useCase.ExecutarAsync(1);

            // Assert
            Assert.True(resultado);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(r =>
                r.RemoverLogico(1, It.IsAny<string?>()),
                Times.Once);
        }
    }
}
