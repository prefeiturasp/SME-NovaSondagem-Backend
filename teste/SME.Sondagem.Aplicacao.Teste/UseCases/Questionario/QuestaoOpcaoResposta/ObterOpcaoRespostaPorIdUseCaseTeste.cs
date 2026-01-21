using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Tests.UseCases.QuestaoOpcaoResposta
{
    public class ObterQuestaoOpcaoRespostaPorIdUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestaoOpcaoResposta> repositorioMock;
        private readonly ObterQuestaoOpcaoRespostaPorIdUseCase useCase;

        public ObterQuestaoOpcaoRespostaPorIdUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioQuestaoOpcaoResposta>();
            useCase = new ObterQuestaoOpcaoRespostaPorIdUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_null_quando_questao_opcao_resposta_nao_existir()
        {
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta?)null);

            var resultado = await useCase.ExecutarAsync(1);

            Assert.Null(resultado);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_dto_mapeado_quando_questao_opcao_resposta_existir()
        {
            var entidade = CriarEntidade();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(entidade.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidade);

            var resultado = await useCase.ExecutarAsync(entidade.Id);

            Assert.NotNull(resultado);

            Assert.Equal(entidade.Id, resultado.Id);
            Assert.Equal(entidade.QuestaoId, resultado.QuestaoId);
            Assert.Equal(entidade.OpcaoRespostaId, resultado.OpcaoRespostaId);
            Assert.Equal(entidade.Ordem, resultado.Ordem);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(entidade.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #region Helpers

        private static SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta CriarEntidade()
        {
            return new SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta(
                questaoId: 10,
                opcaoRespostaId: 20,
                ordem: 3
            );
        }

        #endregion
    }
}
