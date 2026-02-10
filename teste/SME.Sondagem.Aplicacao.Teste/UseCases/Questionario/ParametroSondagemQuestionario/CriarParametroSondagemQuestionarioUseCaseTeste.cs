using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagemQuestionario
{
    public class CriarParametroSondagemQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagemQuestionario> repositorioMock;
        private readonly CriarParametroSondagemQuestionarioUseCase useCase;

        public CriarParametroSondagemQuestionarioUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagemQuestionario>();
            useCase = new CriarParametroSondagemQuestionarioUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_criar_parametro_sondagem_questionario_e_retornar_id_gerado()
        {
            // Arrange
            const long idGerado = 10;

            Dominio.Entidades.ParametroSondagemQuestionario? entidadeSalva = null;

            repositorioMock
                .Setup(r => r.SalvarAsync(It.IsAny<Dominio.Entidades.ParametroSondagemQuestionario>(), It.IsAny<CancellationToken>()))
                .Callback<Dominio.Entidades.ParametroSondagemQuestionario, CancellationToken>((entidade, _) =>
                {
                    entidadeSalva = entidade;
                })
                .ReturnsAsync(idGerado);

            var dto = new ParametroSondagemQuestionarioDto
            {
                IdParametroSondagem = 1,
                IdQuestionario = 2,
                Valor = "valor teste"
            };

            // Act
            var resultado = await useCase.ExecutarAsync(dto);

            // Assert
            Assert.Equal(idGerado, resultado);

            Assert.NotNull(entidadeSalva);
            Assert.Equal(dto.IdParametroSondagem, entidadeSalva!.IdParametroSondagem);
            Assert.Equal(dto.IdQuestionario, entidadeSalva.IdQuestionario);
            Assert.Equal(dto.Valor, entidadeSalva.Valor);

            repositorioMock.Verify(r =>
                r.SalvarAsync(It.IsAny<Dominio.Entidades.ParametroSondagemQuestionario>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
