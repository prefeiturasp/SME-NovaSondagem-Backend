using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Tests.UseCases.QuestaoOpcaoResposta
{
    public class ObterQuestaoOpcaoRespostasUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestaoOpcaoResposta> repositorioMock;
        private readonly ObterQuestaoOpcaoRespostasUseCase useCase;

        public ObterQuestaoOpcaoRespostasUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioQuestaoOpcaoResposta>();
            useCase = new ObterQuestaoOpcaoRespostasUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_lista_de_dtos_mapeados()
        {
            var entidades = new List<SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta>
            {
                new SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta(
                    questaoId: 1,
                    opcaoRespostaId: 10,
                    ordem: 1
                ),
                new SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta(
                    questaoId: 2,
                    opcaoRespostaId: 20,
                    ordem: 2
                )
            };

            repositorioMock
                .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidades);

            var resultado = await useCase.ExecutarAsync();

            Assert.NotNull(resultado);

            var lista = resultado.ToList();
            Assert.Equal(2, lista.Count);

            Assert.Equal(entidades[0].QuestaoId, lista[0].QuestaoId);
            Assert.Equal(entidades[0].OpcaoRespostaId, lista[0].OpcaoRespostaId);
            Assert.Equal(entidades[0].Ordem, lista[0].Ordem);

            Assert.Equal(entidades[1].QuestaoId, lista[1].QuestaoId);
            Assert.Equal(entidades[1].OpcaoRespostaId, lista[1].OpcaoRespostaId);
            Assert.Equal(entidades[1].Ordem, lista[1].Ordem);

            repositorioMock.Verify(
                r => r.ListarAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
