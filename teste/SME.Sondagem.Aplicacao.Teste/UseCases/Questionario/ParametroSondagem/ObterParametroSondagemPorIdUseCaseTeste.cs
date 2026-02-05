using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagem
{
    public class ObterParametroSondagemPorIdUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagem> repositorioMock;
        private readonly ObterParametroSondagemPorIdUseCase useCase;

        public ObterParametroSondagemPorIdUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagem>();
            useCase = new ObterParametroSondagemPorIdUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_null_quando_parametro_nao_existir()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.ParametroSondagem)null!);

            // Act
            var resultado = await useCase.ExecutarAsync(1);

            // Assert
            Assert.Null(resultado);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_dto_quando_parametro_existir()
        {
            // Arrange
            var entidade = new Dominio.Entidades.ParametroSondagem
            {
                Id = 1,
                Nome = "Parametro Teste",
                Descricao = "Descrição Teste",
                Ativo = true,
                Tipo = TipoParametroSondagem.ExibirDescricaoOpcaoResposta,
                CriadoEm = DateTime.Now.AddDays(-1),
                CriadoPor = "Sistema",
                CriadoRF = "123456",
                AlteradoEm = DateTime.Now,
                AlteradoPor = "Admin",
                AlteradoRF = "654321"
            };

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidade);

            // Act
            var resultado = await useCase.ExecutarAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(entidade.Id, resultado.Id);
            Assert.Equal(entidade.Nome, resultado.Nome);
            Assert.Equal(entidade.Descricao, resultado.Descricao);
            Assert.Equal(entidade.Ativo, resultado.Ativo);
            Assert.Equal(entidade.Tipo, resultado.Tipo);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
