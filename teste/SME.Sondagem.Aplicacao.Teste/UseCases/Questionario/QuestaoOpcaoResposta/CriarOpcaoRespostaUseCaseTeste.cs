using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Tests.UseCases.QuestaoOpcaoResposta
{
    public class CriarQuestaoOpcaoRespostaUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestaoOpcaoResposta> repositorioMock;
        private readonly CriarQuestaoOpcaoRespostaUseCase useCase;

        public CriarQuestaoOpcaoRespostaUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioQuestaoOpcaoResposta>();
            useCase = new CriarQuestaoOpcaoRespostaUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_criar_questao_opcao_resposta_e_retornar_id()
        {
            const long idEsperado = 123;

            var dto = new QuestaoOpcaoRespostaDto
            {
                Ordem = 2,
                OpcaoRespostaId = 10,
                QuestaoId = 20
            };

            repositorioMock
                .Setup(r => r.CriarAsync(
                    It.Is<SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta>(q =>
                        q.Ordem == dto.Ordem &&
                        q.OpcaoRespostaId == dto.OpcaoRespostaId &&
                        q.QuestaoId == dto.QuestaoId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(idEsperado);

            var resultado = await useCase.ExecutarAsync(dto);

            Assert.Equal(idEsperado, resultado);

            repositorioMock.Verify(
                r => r.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta>(), It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.VerifyNoOtherCalls();
        }
    }
}
