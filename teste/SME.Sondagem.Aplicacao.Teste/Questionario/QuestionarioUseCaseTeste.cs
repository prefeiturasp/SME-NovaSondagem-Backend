using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Questionario
{
    public class QuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionario> questionarioRepositoryMock;
        private readonly QuestionarioUseCase useCase;

        public QuestionarioUseCaseTeste()
        {
            questionarioRepositoryMock = new Mock<IRepositorioQuestionario>();
            useCase = new QuestionarioUseCase(questionarioRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterQuestionariosAsync_DeveRetornarDadosDoRepositorio()
        {
            var questionarios = new List<object>
            {
                new { Id = 1, Nome = "Questionário A" },
                new { Id = 2, Nome = "Questionário B" }
            };

            questionarioRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(questionarios);

            var resultado = await useCase.ObterQuestionariosAsync();

            Assert.NotNull(resultado);
            Assert.Equal(questionarios, resultado);

            questionarioRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterQuestionariosAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var questionarios = new List<object>();

            questionarioRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(questionarios);

            var resultado = await useCase.ObterQuestionariosAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            questionarioRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}
