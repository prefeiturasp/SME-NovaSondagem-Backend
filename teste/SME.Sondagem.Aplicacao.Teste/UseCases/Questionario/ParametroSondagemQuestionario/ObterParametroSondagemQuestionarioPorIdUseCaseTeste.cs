using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagemQuestionario
{
    public class ObterParametroSondagemQuestionarioPorIdUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagemQuestionario> repositorioMock;
        private readonly ObterParametroSondagemQuestionarioPorIdUseCase useCase;

        public ObterParametroSondagemQuestionarioPorIdUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagemQuestionario>();
            useCase = new ObterParametroSondagemQuestionarioPorIdUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_null_quando_parametro_nao_for_encontrado()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.ParametroSondagemQuestionario)null!);

            // Act
            var resultado = await useCase.ExecutarAsync(1);

            // Assert
            Assert.Null(resultado);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_dto_mapeado_quando_parametro_for_encontrado()
        {
            // Arrange
            var entidade = CriarEntidade();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidade);

            // Act
            var resultado = await useCase.ExecutarAsync(1);

            // Assert
            Assert.NotNull(resultado);

            Assert.Equal(entidade.Id, resultado!.Id);
            Assert.Equal(entidade.IdParametroSondagem, resultado.IdParametroSondagem);
            Assert.Equal(entidade.IdQuestionario, resultado.IdQuestionario);
            Assert.Equal(entidade.Valor, resultado.Valor);
            Assert.Equal(entidade.CriadoEm, resultado.CriadoEm);
            Assert.Equal(entidade.CriadoPor, resultado.CriadoPor);
            Assert.Equal(entidade.CriadoRF, resultado.CriadoRF);
            Assert.Equal(entidade.AlteradoEm, resultado.AlteradoEm);
            Assert.Equal(entidade.AlteradoPor, resultado.AlteradoPor);
            Assert.Equal(entidade.AlteradoRF, resultado.AlteradoRF);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #region Builders

        private static Dominio.Entidades.ParametroSondagemQuestionario CriarEntidade()
        {
            return new Dominio.Entidades.ParametroSondagemQuestionario
            {
                Id = 10,
                IdParametroSondagem = 20,
                IdQuestionario = 30,
                Valor = "valor teste",
                CriadoEm = DateTime.UtcNow.AddDays(-1),
                CriadoPor = "usuario",
                CriadoRF = "123456",
                AlteradoEm = DateTime.UtcNow,
                AlteradoPor = "usuario alteracao",
                AlteradoRF = "654321"
            };
        }

        #endregion
    }
}
