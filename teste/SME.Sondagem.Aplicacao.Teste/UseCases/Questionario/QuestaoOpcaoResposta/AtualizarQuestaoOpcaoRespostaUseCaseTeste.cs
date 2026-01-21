using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Tests.UseCases.QuestaoOpcaoResposta
{
    public class AtualizarQuestaoOpcaoRespostaUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestaoOpcaoResposta> repositorioMock;
        private readonly AtualizarQuestaoOpcaoRespostaUseCase useCase;

        public AtualizarQuestaoOpcaoRespostaUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioQuestaoOpcaoResposta>();
            useCase = new AtualizarQuestaoOpcaoRespostaUseCase(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_null_quando_questao_opcao_resposta_nao_existir()
        {
            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta?)null);

            var dto = new QuestaoOpcaoRespostaDto();

            var resultado = await useCase.ExecutarAsync(1, dto);

            Assert.Null(resultado);

            repositorioMock.Verify(
                r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioMock.Verify(
                r => r.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.QuestaoOpcaoResposta>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_retornar_null_quando_atualizacao_falhar()
        {
            var entidade = CriarEntidade();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidade);

            repositorioMock
                .Setup(r => r.AtualizarAsync(entidade, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var dto = CriarDto();

            var resultado = await useCase.ExecutarAsync(entidade.Id, dto);

            Assert.Null(resultado);

            repositorioMock.Verify(
                r => r.AtualizarAsync(entidade, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_atualizar_e_retornar_dto_quando_sucesso()
        {
            var entidade = CriarEntidade();

            repositorioMock
                .Setup(r => r.ObterPorIdAsync(entidade.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entidade);

            repositorioMock
                .Setup(r => r.AtualizarAsync(entidade, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var dto = CriarDto();

            var resultado = await useCase.ExecutarAsync(entidade.Id, dto);

            Assert.NotNull(resultado);
            Assert.Equal(entidade.Id, resultado.Id);
            Assert.Equal(dto.QuestaoId, resultado.QuestaoId);
            Assert.Equal(dto.OpcaoRespostaId, resultado.OpcaoRespostaId);
            Assert.Equal(dto.Ordem, resultado.Ordem);

            repositorioMock.VerifyAll();
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

        private static QuestaoOpcaoRespostaDto CriarDto()
        {
            return new QuestaoOpcaoRespostaDto
            {
                QuestaoId = 10,
                OpcaoRespostaId = 20,
                Ordem = 3
            };
        }

        #endregion
    }
}
