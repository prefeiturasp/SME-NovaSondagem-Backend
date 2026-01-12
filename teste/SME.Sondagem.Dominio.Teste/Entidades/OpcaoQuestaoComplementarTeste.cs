using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class OpcaoQuestaoComplementarTeste
    {
        [Fact]
        public void Deve_criar_opcao_questao_complementar_com_ids()
        {
            long opcaoRespostaId = 10;
            long questaoComplementarId = 20;

            var entidade = new OpcaoQuestaoComplementar(opcaoRespostaId, questaoComplementarId);

            Assert.Equal(opcaoRespostaId, entidade.OpcaoRespostaId);
            Assert.Equal(questaoComplementarId, entidade.QuestaoComplementarId);
        }

        [Fact]
        public void Deve_possuir_navegacoes_para_ef_core()
        {
            var entidade = CriarEntidadePadrao();

            Assert.Null(entidade.OpcaoResposta);
            Assert.Null(entidade.QuestaoComplementar);
        }

        private static OpcaoQuestaoComplementar CriarEntidadePadrao()
        {
            return new OpcaoQuestaoComplementar(
                opcaoRespostaId: 1,
                questaoComplementarId: 2
            );
        }
    }
}
