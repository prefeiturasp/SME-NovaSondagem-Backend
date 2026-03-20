using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagemQuestionario
{
    public class ObterParametrosSondagemQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagemQuestionario> repositorioMock;
        private readonly ObterParametrosSondagemQuestionarioUseCase useCase;

        public ObterParametrosSondagemQuestionarioUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagemQuestionario>();
            useCase = new ObterParametrosSondagemQuestionarioUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_lista_vazia_quando_nao_existirem_parametros()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.ParametroSondagemQuestionario>());

            // Act
            var resultado = await useCase.ExecutarAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            repositorioMock.Verify(r =>
                r.ListarAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_lista_de_dtos_mapeados_quando_existirem_parametros()
        {
            // Arrange
            var entidades = new List<Dominio.Entidades.ParametroSondagemQuestionario>
            {
                CriarEntidade(1),
                CriarEntidade(2)
            };

            repositorioMock
                .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidades);

            // Act
            var resultado = (await useCase.ExecutarAsync()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);

            for (int i = 0; i < entidades.Count; i++)
            {
                var entidade = entidades[i];
                var dto = resultado[i];

                Assert.Equal(entidade.Id, dto.Id);
                Assert.Equal(entidade.IdParametroSondagem, dto.IdParametroSondagem);
                Assert.Equal(entidade.IdQuestionario, dto.IdQuestionario);
                Assert.Equal(entidade.Valor, dto.Valor);
            }

            repositorioMock.Verify(r =>
                r.ListarAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #region Builders

        private static Dominio.Entidades.ParametroSondagemQuestionario CriarEntidade(long id)
        {
            return new Dominio.Entidades.ParametroSondagemQuestionario
            {
                Id = (int)id,
                IdParametroSondagem = 100 + (int)id,
                IdQuestionario = 200 + (int)id,
                Valor = $"valor {id}",
                CriadoEm = DateTime.UtcNow.AddDays(-id),
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
