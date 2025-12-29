using Moq;
using SME.Sondagem.Aplicacao.UseCases.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Ciclo
{
    public class CicloUseCaseTeste
    {
        private readonly Mock<IRepositorioCiclo> cicloRepositoryMock;
        private readonly CicloUseCase useCase;

        public CicloUseCaseTeste()
        {
            cicloRepositoryMock = new Mock<IRepositorioCiclo>();
            useCase = new CicloUseCase(cicloRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterCiclosAsync_DeveRetornarDadosDoRepositorio()
        {
            var ciclos = new List<object>
            {
                new { Id = 1, Nome = "Ciclo 1" },
                new { Id = 2, Nome = "Ciclo 2" }
            };

            cicloRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(ciclos);

            var resultado = await useCase.ObterCiclosAsync();

            Assert.NotNull(resultado);
            Assert.Equal(ciclos, resultado);

            cicloRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterCiclosAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var ciclos = new List<object>();

            cicloRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(ciclos);

            var resultado = await useCase.ObterCiclosAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            cicloRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}
