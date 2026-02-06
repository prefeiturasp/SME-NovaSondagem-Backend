using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagemQuestionario
{
    public class ObterParametroSondagemQuestionarioPorIdQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagemQuestionario> repositorioMock;
        private readonly ObterParametroSondagemQuestionarioPorIdQuestionarioUseCase useCase;

        public ObterParametroSondagemQuestionarioPorIdQuestionarioUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagemQuestionario>();
            useCase = new ObterParametroSondagemQuestionarioPorIdQuestionarioUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_parametros_do_questionario_quando_existirem()
        {
            // Arrange
            const int idQuestionario = 10;

            var parametros = new List<Dominio.Entidades.ParametroSondagemQuestionario>
            {
                new()
                {
                    Id = 1,
                    IdQuestionario = idQuestionario,
                    Valor = "A",
                    ParametroSondagem = new Dominio.Entidades.ParametroSondagem
                    {
                        Tipo = Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem
                    }
                },
                new()
                {
                    Id = 2,
                    IdQuestionario = idQuestionario,
                    Valor = "B",
                    ParametroSondagem = new Dominio.Entidades.ParametroSondagem
                    {
                        Tipo = Dominio.Enums.TipoParametroSondagem.ExibirDescricaoOpcaoResposta
                    }
                }
            };

            repositorioMock
                .Setup(r => r.ObterPorIdQuestionarioAsync(idQuestionario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametros);

            // Act
            var resultado = await useCase.ExecutarAsync(idQuestionario);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());

            var primeiro = resultado.First();
            Assert.Equal(1, primeiro.Id);
            Assert.Equal(idQuestionario, primeiro.IdQuestionario);
            Assert.Equal("A", primeiro.Valor);
            Assert.Equal(
                (int)Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem,
                primeiro.Tipo
            );

            repositorioMock.Verify(
                r => r.ObterPorIdQuestionarioAsync(idQuestionario, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Deve_retornar_lista_vazia_quando_nao_existirem_parametros_para_o_questionario()
        {
            // Arrange
            const long idQuestionario = 99;

            repositorioMock
                .Setup(r => r.ObterPorIdQuestionarioAsync(idQuestionario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.ParametroSondagemQuestionario>());

            // Act
            var resultado = await useCase.ExecutarAsync(idQuestionario);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            repositorioMock.Verify(
                r => r.ObterPorIdQuestionarioAsync(idQuestionario, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
