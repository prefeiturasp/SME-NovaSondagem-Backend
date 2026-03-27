using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagem
{
    public class ObterParametrosSondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagem> repositorioMock;
        private readonly ObterParametrosSondagemUseCase useCase;

        public ObterParametrosSondagemUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagem>();
            useCase = new ObterParametrosSondagemUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_lista_de_parametros_quando_existirem_registros()
        {
            // Arrange
            var parametros = new List<Dominio.Entidades.ParametroSondagem>
            {
                new()
                {
                    Id = 1,
                    Nome = "Parametro 1",
                    Descricao = "Descricao 1",
                    Ativo = true,
                    Tipo = Dominio.Enums.TipoParametroSondagem.ExibirDescricaoOpcaoResposta,
                    CriadoEm = DateTime.Now
                },
                new()
                {
                    Id = 2,
                    Nome = "Parametro 2",
                    Descricao = "Descricao 2",
                    Ativo = false,
                    Tipo = Dominio.Enums.TipoParametroSondagem.OrdenacaoLinguaPortuguesaSegundaLingua,
                    CriadoEm = DateTime.Now
                }
            };

            repositorioMock
                .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametros);

            // Act
            var resultado = await useCase.ExecutarAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());

            var primeiro = resultado.First();
            Assert.Equal(1, primeiro.Id);
            Assert.Equal("Parametro 1", primeiro.Nome);
            Assert.Equal("Descricao 1", primeiro.Descricao);
            Assert.True(primeiro.Ativo);

            repositorioMock.Verify(
                r => r.ListarAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Deve_retornar_lista_vazia_quando_nao_existirem_registros()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.ParametroSondagem>());

            // Act
            var resultado = await useCase.ExecutarAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            repositorioMock.Verify(
                r => r.ListarAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
