using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Tests.UseCases.QuestaoOpcaoResposta
{
    public class ExcluirQuestaoOpcaoRespostaUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestaoOpcaoResposta> repositorioMock;
        private readonly ExcluirQuestaoOpcaoRespostaUseCase useCase;

        public ExcluirQuestaoOpcaoRespostaUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioQuestaoOpcaoResposta>();
            useCase = new ExcluirQuestaoOpcaoRespostaUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_registro_nao_existir()
        {
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta?)null);

            var resultado = await useCase.ExecutarAsync(1);

            Assert.False(resultado);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(
                r => r.ExcluirAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_excluir_e_retornar_true_quando_registro_existir()
        {
            var entidade = CriarEntidade();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidade);

            repositorioMock
                .Setup(r => r.ExcluirAsync(entidade.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await useCase.ExecutarAsync(entidade.Id);

            Assert.True(resultado);

            repositorioMock.Verify(
                r => r.ExcluirAsync(entidade.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #region Helpers

        private static SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta CriarEntidade()
        {
            return new SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta(
                questaoId: 1,
                opcaoRespostaId: 2,
                ordem: 1
            );
        }

        #endregion
    }
}
