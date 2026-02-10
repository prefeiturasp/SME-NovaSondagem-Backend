using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagem
{
    public class CriarParametroSondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagem> repositorioMock;
        private readonly CriarParametroSondagemUseCase useCase;

        public CriarParametroSondagemUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagem>();
            useCase = new CriarParametroSondagemUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_criar_parametro_sondagem_e_retornar_id_gerado()
        {
            // Arrange
            const long idGerado = 42;
            Dominio.Entidades.ParametroSondagem? entidadeSalva = null;

            repositorioMock
                .Setup(r => r.SalvarAsync(It.IsAny<Dominio.Entidades.ParametroSondagem>(), It.IsAny<CancellationToken>()))
                .Callback<Dominio.Entidades.ParametroSondagem, CancellationToken>((entidade, _) =>
                {
                    entidadeSalva = entidade;
                })
                .ReturnsAsync(idGerado);

            var dto = new ParametroSondagemDto
            {
                Nome = "Parâmetro Teste",
                Descricao = "Descrição teste",
                Ativo = true,
                Tipo = TipoParametroSondagem.ExibirTituloTabelaSondagem
            };

            // Act
            var resultado = await useCase.ExecutarAsync(dto);

            // Assert
            Assert.Equal(idGerado, resultado);

            Assert.NotNull(entidadeSalva);
            Assert.Equal(dto.Nome, entidadeSalva!.Nome);
            Assert.Equal(dto.Descricao, entidadeSalva.Descricao);
            Assert.Equal(dto.Ativo, entidadeSalva.Ativo);
            Assert.Equal(dto.Tipo, entidadeSalva.Tipo);

            repositorioMock.Verify(r =>
                r.SalvarAsync(It.IsAny<Dominio.Entidades.ParametroSondagem>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
