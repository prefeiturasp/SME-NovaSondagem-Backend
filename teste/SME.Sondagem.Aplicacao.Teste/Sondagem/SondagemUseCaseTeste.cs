using Moq;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Sondagem
{
    public class SondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioSondagem> sondagemRepositoryMock;
        private readonly SondagemUseCase useCase;

        public SondagemUseCaseTeste()
        {
            sondagemRepositoryMock = new Mock<IRepositorioSondagem>();
            useCase = new SondagemUseCase(sondagemRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterTodasSondagensAsync_DeveRetornarDadosDoRepositorio()
        {
            var sondagens = new List<Dominio.Entidades.Sondagem.Sondagem>
            {
                new Dominio.Entidades.Sondagem.Sondagem("Teste", DateTime.Now) { Id = 1},
                new Dominio.Entidades.Sondagem.Sondagem("Teste", DateTime.Now) { Id = 2}
            };

            sondagemRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(sondagens);

            var resultado = await useCase.ObterTodasSondagensAsync();

            Assert.NotNull(resultado);
            Assert.Equal(sondagens, resultado);

            sondagemRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterTodasSondagensAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var sondagens = new List<Dominio.Entidades.Sondagem.Sondagem>();

            sondagemRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(sondagens);

            var resultado = await useCase.ObterTodasSondagensAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            sondagemRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}
