using Moq;
using SME.Sondagem.Aplicacao.UseCases.Aluno;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Aluno
{
    public class AlunoUseCaseTeste
    {
        private readonly Mock<IRepositorioAluno> alunoRepositoryMock;
        private readonly AlunoUseCase useCase;

        public AlunoUseCaseTeste()
        {
            alunoRepositoryMock = new Mock<IRepositorioAluno>();
            useCase = new AlunoUseCase(alunoRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterAlunosAsync_DeveRetornarDadosDoRepositorio()
        {
            var alunos = new List<object>
            {
                new { Id = 1, Nome = "João" },
                new { Id = 2, Nome = "Maria" }
            };

            alunoRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(alunos);

            var resultado = await useCase.ObterAlunosAsync();

            Assert.NotNull(resultado);
            Assert.Equal(alunos, resultado);

            alunoRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterAlunosAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var alunos = new List<object>();

            alunoRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(alunos);

            var resultado = await useCase.ObterAlunosAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            alunoRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}