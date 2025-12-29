using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Questionario
{
    public class OpcaoRespostaUseCaseTeste
    {
        private readonly Mock<IRepositorioOpcaoResposta> opcaoRespostaRepositoryMock;
        private readonly OpcaoRespostaUseCase useCase;

        public OpcaoRespostaUseCaseTeste()
        {
            opcaoRespostaRepositoryMock = new Mock<IRepositorioOpcaoResposta>();
            useCase = new OpcaoRespostaUseCase(opcaoRespostaRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterOpcoesRespostaAsync_DeveRetornarDadosDoRepositorio()
        {
            var opcoesResposta = new List<object>
            {
                new { Id = 1, Descricao = "Sim" },
                new { Id = 2, Descricao = "Não" }
            };

            opcaoRespostaRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(opcoesResposta);

            var resultado = await useCase.ObterOpcoesRespostaAsync();

            Assert.NotNull(resultado);
            Assert.Equal(opcoesResposta, resultado);

            opcaoRespostaRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ObterOpcoesRespostaAsync_DeveRetornarListaVazia_QuandoRepositorioNaoPossuirDados()
        {
            var opcoesResposta = new List<object>();

            opcaoRespostaRepositoryMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(opcoesResposta);

            var resultado = await useCase.ObterOpcoesRespostaAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            opcaoRespostaRepositoryMock.Verify(
                r => r.ObterTodosAsync(),
                Times.Once);
        }
    }
}
