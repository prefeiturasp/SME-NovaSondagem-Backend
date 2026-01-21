using Moq;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Sondagem
{
    public class AtualizarSondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioSondagem> repositorioMock;
        private readonly AtualizarSondagemUseCase useCase;

        public AtualizarSondagemUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioSondagem>();
            useCase = new AtualizarSondagemUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarNull_QuandoSondagemNaoExistir()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.Sondagem.Sondagem?)null);

            var dto = new SondagemDto
            {
                Descricao = "Nova descrição",
                DataAplicacao = DateTime.Today
            };

            var resultado = await useCase.ExecutarAsync(1, dto);

            Assert.Null(resultado);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(
                r => r.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarNull_QuandoAtualizacaoFalhar()
        {
            var sondagem = CriarSondagem();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(sondagem.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sondagem);

            repositorioMock
                .Setup(r => r.AtualizarAsync(sondagem, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var dto = new SondagemDto
            {
                Descricao = "Descrição alterada",
                DataAplicacao = DateTime.Today.AddDays(1)
            };

            var resultado = await useCase.ExecutarAsync(sondagem.Id, dto);

            Assert.Null(resultado);

            repositorioMock.Verify(
                r => r.AtualizarAsync(sondagem, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveAtualizarSondagemERetornarDto_QuandoSucesso()
        {
            var sondagem = CriarSondagem();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(sondagem.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sondagem);

            repositorioMock
                .Setup(r => r.AtualizarAsync(sondagem, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var dto = new SondagemDto
            {
                Descricao = "Descrição atualizada",
                DataAplicacao = DateTime.Today.AddDays(2)
            };

            var resultado = await useCase.ExecutarAsync(sondagem.Id, dto);

            Assert.NotNull(resultado);
            Assert.Equal(sondagem.Id, resultado.Id);
            Assert.Equal(dto.Descricao, resultado.Descricao);
            Assert.Equal(dto.DataAplicacao, resultado.DataAplicacao);
            Assert.Equal(sondagem.CriadoEm, resultado.CriadoEm);
            Assert.Equal(sondagem.CriadoPor, resultado.CriadoPor);
            Assert.Equal(sondagem.CriadoRF, resultado.CriadoRF);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(sondagem.Id, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(
                r => r.AtualizarAsync(sondagem, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem CriarSondagem()
        {
            return new SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem(
                descricao: "Descrição original",
                dataAplicacao: DateTime.Today
            );
        }
    }
}
