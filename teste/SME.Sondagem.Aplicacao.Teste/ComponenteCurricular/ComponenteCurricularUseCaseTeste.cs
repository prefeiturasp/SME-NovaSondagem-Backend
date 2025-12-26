using Moq;
using SME.Sondagem.Aplicacao.UseCases.ComponenteCurricular;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.ComponenteCurricular
{
    public class ComponenteCurricularUseCaseTeste
    {
        private readonly Mock<IComponenteCurricularRepository> componenteRepositoryMock;
        private readonly ComponenteCurricularUseCase useCase;

        public ComponenteCurricularUseCaseTeste()
        {
            componenteRepositoryMock = new Mock<IComponenteCurricularRepository>();
            useCase = new ComponenteCurricularUseCase(componenteRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterComponentesAsync_DeveRetornarDadosDoRepositorio()
        {
            var componentes = new List<object>
            {
                new { Id = 1, Nome = "Matemática" },
                new { Id = 2, Nome = "Português" }
            };

            componenteRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(componentes);

            var resultado = await useCase.ObterComponentesAsync();

            Assert.NotNull(resultado);
            Assert.Equal(componentes, resultado);

            componenteRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterComponentesAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var componentes = new List<object>();

            componenteRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(componentes);

            var resultado = await useCase.ObterComponentesAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            componenteRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}
