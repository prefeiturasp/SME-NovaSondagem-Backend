using Moq;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Sondagem
{
    public class ObterSondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioSondagem> repositorioMock;
        private readonly ObterSondagemUseCase useCase;

        public ObterSondagemUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioSondagem>();
            useCase = new ObterSondagemUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoNaoExistiremSondagens()
        {
            repositorioMock
                .Setup(r => r.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem?>());

            var resultado = await useCase.ExecutarAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            repositorioMock.Verify(
                r => r.ObterTodosAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarListaMapeadaCorretamente_QuandoExistiremSondagens()
        {
            var sondagens = CriarListaSondagens();

            repositorioMock
                .Setup(r => r.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(sondagens);

            var resultado = (await useCase.ExecutarAsync()).ToList();

            Assert.Equal(2, resultado.Count);

            for (int i = 0; i < sondagens.Count; i++)
            {
                Assert.Equal(sondagens[i].Id, resultado[i].Id);
                Assert.Equal(sondagens[i].Descricao, resultado[i].Descricao);
                Assert.Equal(sondagens[i].DataAplicacao, resultado[i].DataAplicacao);
                Assert.Equal(sondagens[i].CriadoEm, resultado[i].CriadoEm);
                Assert.Equal(sondagens[i].CriadoPor, resultado[i].CriadoPor);
                Assert.Equal(sondagens[i].CriadoRF, resultado[i].CriadoRF);
                Assert.Equal(sondagens[i].AlteradoEm, resultado[i].AlteradoEm);
                Assert.Equal(sondagens[i].AlteradoPor, resultado[i].AlteradoPor);
                Assert.Equal(sondagens[i].AlteradoRF, resultado[i].AlteradoRF);
            }

            repositorioMock.Verify(
                r => r.ObterTodosAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static List<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem> CriarListaSondagens()
        {
            return new List<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem>
            {
                new SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem(
                    descricao: "Sondagem Português",
                    dataAplicacao: new DateTime(2026, 1, 10)
                )
                {
                    AlteradoEm = new DateTime(2026, 1, 5),
                    AlteradoPor = "Editor 1",
                    AlteradoRF = "222222"
                },

                new SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem(
                    descricao: "Sondagem Matemática",
                    dataAplicacao: new DateTime(2026, 2, 10)
                )
            };
        }
    }
}
