using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Questionario
{
    public class QuestaoUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestao> questaoRepositoryMock;
        private readonly QuestaoUseCase useCase;

        public QuestaoUseCaseTeste()
        {
            questaoRepositoryMock = new Mock<IRepositorioQuestao>();
            useCase = new QuestaoUseCase(questaoRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterQuestoesAsync_DeveRetornarDadosDoRepositorio()
        {
            var questoes = new List<object>
            {
                new { Id = 1, Descricao = "Questão 1" },
                new { Id = 2, Descricao = "Questão 2" }
            };

            questaoRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(questoes);

            var resultado = await useCase.ObterQuestoesAsync();

            Assert.NotNull(resultado);
            Assert.Equal(questoes, resultado);

            questaoRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterQuestoesAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var questoes = new List<object>();

            questaoRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(questoes);

            var resultado = await useCase.ObterQuestoesAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            questaoRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}
