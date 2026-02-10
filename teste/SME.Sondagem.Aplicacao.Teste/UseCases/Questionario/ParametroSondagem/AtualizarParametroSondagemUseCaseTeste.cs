using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagem
{
    public class AtualizarParametroSondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagem> repositorioMock;
        private readonly AtualizarParametroSondagemUseCase useCase;

        public AtualizarParametroSondagemUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagem>();
            useCase = new AtualizarParametroSondagemUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_null_quando_parametro_nao_for_encontrado()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.ParametroSondagem)null!);

            var dto = CriarDto();

            // Act
            var resultado = await useCase.ExecutarAsync(1, dto);

            // Assert
            Assert.Null(resultado);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Deve_retornar_null_quando_salvar_async_retornar_zero()
        {
            // Arrange
            var entidade = CriarEntidade();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidade);

            repositorioMock
                .Setup(r => r.SalvarAsync(entidade, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var dto = CriarDto();

            // Act
            var resultado = await useCase.ExecutarAsync(1, dto);

            // Assert
            Assert.Null(resultado);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(r =>
                r.SalvarAsync(entidade, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_atualizar_parametro_e_retornar_dto_quando_salvar_com_sucesso()
        {
            // Arrange
            var entidade = CriarEntidade();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidade);

            repositorioMock
                .Setup(r => r.SalvarAsync(entidade, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = CriarDto();

            // Act
            var resultado = await useCase.ExecutarAsync(1, dto);

            // Assert
            Assert.NotNull(resultado);

            Assert.Equal(entidade.Id, resultado!.Id);
            Assert.Equal(dto.Nome, resultado.Nome);
            Assert.Equal(dto.Descricao, resultado.Descricao);
            Assert.Equal(dto.Ativo, resultado.Ativo);
            Assert.Equal(dto.Tipo, resultado.Tipo);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(r =>
                r.SalvarAsync(entidade, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #region Builders

        private static Dominio.Entidades.ParametroSondagem CriarEntidade()
        {
            return new Dominio.Entidades.ParametroSondagem
            {
                Id = 10,
                Nome = "Nome antigo",
                Descricao = "Descrição antiga",
                Ativo = false,
                Tipo = TipoParametroSondagem.OrdenacaoLinguaPortuguesaSegundaLingua,
                CriadoEm = DateTime.UtcNow.AddDays(-10),
                CriadoPor = "usuario",
                CriadoRF = "123456",
                AlteradoEm = DateTime.UtcNow,
                AlteradoPor = "usuario alteracao",
                AlteradoRF = "654321"
            };
        }

        private static ParametroSondagemDto CriarDto()
        {
            return new ParametroSondagemDto
            {
                Nome = "Nome novo",
                Descricao = "Descrição nova",
                Ativo = true,
                Tipo = TipoParametroSondagem.ExibirTituloTabelaSondagem
            };
        }

        #endregion
    }
}
