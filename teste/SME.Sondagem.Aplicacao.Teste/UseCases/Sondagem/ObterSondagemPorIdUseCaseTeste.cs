using Moq;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Sondagem
{
    public class ObterSondagemPorIdUseCaseTeste
    {
        private readonly Mock<IRepositorioSondagem> repositorioMock;
        private readonly ObterSondagemPorIdUseCase useCase;

        public ObterSondagemPorIdUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioSondagem>();
            useCase = new ObterSondagemPorIdUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarNull_QuandoSondagemNaoExistir()
        {
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem?)null);

            var resultado = await useCase.ExecutarAsync(1);

            Assert.Null(resultado);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarDto_MapeadoCorretamente_QuandoSondagemExistir()
        {
            var sondagem = CriarSondagemCompleta();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(sondagem.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sondagem);

            var resultado = await useCase.ExecutarAsync(sondagem.Id);

            Assert.NotNull(resultado);

            Assert.Equal(sondagem.Id, resultado.Id);
            Assert.Equal(sondagem.Descricao, resultado.Descricao);
            Assert.Equal(sondagem.DataAplicacao, resultado.DataAplicacao);
            Assert.Equal(sondagem.CriadoEm, resultado.CriadoEm);
            Assert.Equal(sondagem.CriadoPor, resultado.CriadoPor);
            Assert.Equal(sondagem.CriadoRF, resultado.CriadoRF);
            Assert.Equal(sondagem.AlteradoEm, resultado.AlteradoEm);
            Assert.Equal(sondagem.AlteradoPor, resultado.AlteradoPor);
            Assert.Equal(sondagem.AlteradoRF, resultado.AlteradoRF);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(sondagem.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem CriarSondagemCompleta()
        {
            return new SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem(
                descricao: "Sondagem Matemática",
                dataAplicacao: new DateTime(2026, 1, 15)
            )
            {
                AlteradoEm = new DateTime(2026, 1, 10),
                AlteradoPor = "Usuário Alteração",
                AlteradoRF = "654321"
            };
        }
    }
}
