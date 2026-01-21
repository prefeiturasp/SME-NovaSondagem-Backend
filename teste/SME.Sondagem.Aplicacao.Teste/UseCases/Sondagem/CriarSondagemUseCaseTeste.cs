using Moq;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Sondagem
{
    public class CriarSondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioSondagem> repositorioMock;
        private readonly CriarSondagemUseCase useCase;
        public CriarSondagemUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioSondagem>();
            useCase = new CriarSondagemUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task ExecutarAsync_DeveCriarSondagem_InserirNoRepositorio_ERetornarId()
        {
            var dto = new SondagemDto
            {
                Descricao = "Sondagem diagnóstica",
                DataAplicacao = new DateTime(2026, 1, 20)
            };

            SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem? sondagemInserida = null;

            repositorioMock
                .Setup(r => r.InserirAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem>(), It.IsAny<CancellationToken>()))
                .Callback<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem, CancellationToken>((s, _) => sondagemInserida = s)
                .Returns(Task.CompletedTask);

            var resultado = await useCase.ExecutarAsync(dto);

            Assert.NotNull(sondagemInserida);
            Assert.Equal(dto.Descricao, sondagemInserida.Descricao);
            Assert.Equal(dto.DataAplicacao, sondagemInserida.DataAplicacao);

            Assert.Equal(sondagemInserida.Id, resultado);

            repositorioMock.Verify(
                r => r.InserirAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DevePropagarCancellationToken()
        {
            var dto = new SondagemDto
            {
                Descricao = "Teste cancellation",
                DataAplicacao = DateTime.Today
            };

            var cancellationToken = new CancellationTokenSource().Token;

            repositorioMock
                .Setup(r => r.InserirAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem>(), cancellationToken))
                .Returns(Task.CompletedTask);

            await useCase.ExecutarAsync(dto, cancellationToken);

            repositorioMock.Verify(
                r => r.InserirAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem>(), cancellationToken),
                Times.Once);
        }
    }
}
