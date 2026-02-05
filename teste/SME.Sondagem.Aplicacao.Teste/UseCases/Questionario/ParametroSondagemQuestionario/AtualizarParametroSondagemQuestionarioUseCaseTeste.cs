using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagemQuestionario
{
    public class AtualizarParametroSondagemQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagemQuestionario> repositorioMock;
        private readonly AtualizarParametroSondagemQuestionarioUseCase useCase;

        public AtualizarParametroSondagemQuestionarioUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagemQuestionario>();
            useCase = new AtualizarParametroSondagemQuestionarioUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_null_quando_parametro_nao_for_encontrado()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.ParametroSondagemQuestionario)null!);

            // Act
            var resultado = await useCase.ExecutarAsync(1, new ParametroSondagemQuestionarioDto());

            // Assert
            Assert.Null(resultado);

            repositorioMock.Verify(r =>
                r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()),
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

            repositorioMock.Verify(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            repositorioMock.Verify(r => r.SalvarAsync(entidade, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_atualizar_e_retornar_dto_quando_salvar_com_sucesso()
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
            Assert.Equal(dto.IdParametroSondagem, resultado.IdParametroSondagem);
            Assert.Equal(dto.IdQuestionario, resultado.IdQuestionario);
            Assert.Equal(dto.Valor, resultado.Valor);
            Assert.Equal(entidade.CriadoEm, resultado.CriadoEm);
            Assert.Equal(entidade.CriadoPor, resultado.CriadoPor);
            Assert.Equal(entidade.CriadoRF, resultado.CriadoRF);
            Assert.Equal(entidade.AlteradoEm, resultado.AlteradoEm);
            Assert.Equal(entidade.AlteradoPor, resultado.AlteradoPor);
            Assert.Equal(entidade.AlteradoRF, resultado.AlteradoRF);

            repositorioMock.Verify(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            repositorioMock.Verify(r => r.SalvarAsync(entidade, It.IsAny<CancellationToken>()), Times.Once);
        }

        #region Builders

        private static Dominio.Entidades.ParametroSondagemQuestionario CriarEntidade()
        {
            return new Dominio.Entidades.ParametroSondagemQuestionario(
                idParametroSondagem: 1,
                idQuestionario: 2,
                valor: "valor antigo",
                criadoPor: "usuario",
                criadoRF: "123456"
            );
        }

        private static ParametroSondagemQuestionarioDto CriarDto()
        {
            return new ParametroSondagemQuestionarioDto
            {
                IdParametroSondagem = 10,
                IdQuestionario = 20,
                Valor = "valor atualizado"
            };
        }

        #endregion
    }
}
