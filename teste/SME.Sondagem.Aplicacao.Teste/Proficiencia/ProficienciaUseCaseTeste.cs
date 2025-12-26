using Moq;
using SME.Sondagem.Aplicacao.UseCases.Aluno;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Proficiencia
{
    public class ProficienciaUseCaseTeste
    {
        private readonly Mock<IRepositorioProficiencia> repositorioProficienciaMock;
        private readonly ProficienciaUseCase useCase;

        public ProficienciaUseCaseTeste()
        {
            repositorioProficienciaMock = new Mock<IRepositorioProficiencia>();
            useCase = new ProficienciaUseCase(repositorioProficienciaMock.Object);
        }

        [Fact]
        public async Task ObterProficienciasAsync_DeveRetornarDadosDoRepositorio()
        {
            var proficiencias = new List<object>
            {
                new { Id = 1, Descricao = "Abaixo do básico" },
                new { Id = 2, Descricao = "Adequado" }
            };

            repositorioProficienciaMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(proficiencias);

            var resultado = await useCase.ObterProficienciasAsync();
            
            Assert.NotNull(resultado);
            Assert.Equal(proficiencias, resultado);

            repositorioProficienciaMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterProficienciasAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var proficiencias = new List<object>();

            repositorioProficienciaMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(proficiencias);

            var resultado = await useCase.ObterProficienciasAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            repositorioProficienciaMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}
