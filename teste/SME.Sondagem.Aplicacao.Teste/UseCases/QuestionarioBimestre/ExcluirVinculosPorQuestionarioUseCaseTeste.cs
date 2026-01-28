using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestionarioBimestre;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.QuestionarioBimestre
{
    public class ExcluirVinculosPorQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionarioBimestre> _repositorioMock;
        private readonly ExcluirVinculosPorQuestionarioUseCase _useCase;

        public ExcluirVinculosPorQuestionarioUseCaseTeste()
        {
            _repositorioMock = new Mock<IRepositorioQuestionarioBimestre>();
            _useCase = new ExcluirVinculosPorQuestionarioUseCase(_repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Excluir_Vinculos_Com_Sucesso()
        {
            var questionarioId = 10;

            _repositorioMock
                .Setup(x => x.ExcluirPorQuestionarioIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await _useCase.ExecutarAsync(questionarioId);

            Assert.True(resultado);
            _repositorioMock.Verify(x => x.ExcluirPorQuestionarioIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_False_Quando_Nao_Houver_Vinculos()
        {
            var questionarioId = 999;

            _repositorioMock
                .Setup(x => x.ExcluirPorQuestionarioIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resultado = await _useCase.ExecutarAsync(questionarioId);

            Assert.False(resultado);
        }

        [Fact]
        public async Task Deve_Passar_CancellationToken_Para_Repositorio()
        {
            var questionarioId = 5;
            var cancellationToken = new CancellationToken();

            _repositorioMock
                .Setup(x => x.ExcluirPorQuestionarioIdAsync(questionarioId, cancellationToken))
                .ReturnsAsync(true);

            await _useCase.ExecutarAsync(questionarioId, cancellationToken);

            _repositorioMock.Verify(x => x.ExcluirPorQuestionarioIdAsync(questionarioId, cancellationToken), Times.Once);
        }
    }
}