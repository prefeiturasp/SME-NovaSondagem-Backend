using Moq;
using SME.Sondagem.Aplicacao.UseCases.Aluno;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Proficiencia
{
    public class ProficienciaUseCaseTeste
    {
        private readonly Mock<IProficienciaRepository> proficienciaRepositoryMock;
        private readonly ProficienciaUseCase useCase;

        public ProficienciaUseCaseTeste()
        {
            proficienciaRepositoryMock = new Mock<IProficienciaRepository>();
            useCase = new ProficienciaUseCase(proficienciaRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterProficienciasAsync_DeveRetornarDadosDoRepositorio()
        {
            var proficiencias = new List<object>
            {
                new { Id = 1, Descricao = "Abaixo do básico" },
                new { Id = 2, Descricao = "Adequado" }
            };

            proficienciaRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(proficiencias);

            var resultado = await useCase.ObterProficienciasAsync();
            
            Assert.NotNull(resultado);
            Assert.Equal(proficiencias, resultado);

            proficienciaRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterProficienciasAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var proficiencias = new List<object>();

            proficienciaRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(proficiencias);

            var resultado = await useCase.ObterProficienciasAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            proficienciaRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}
