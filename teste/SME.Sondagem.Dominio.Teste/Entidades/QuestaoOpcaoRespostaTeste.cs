using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class QuestaoOpcaoRespostaTeste
    {
        [Fact]
        public void Deve_criar_questao_opcao_resposta_com_dados_informados()
        {
            var questaoId = 1;
            var opcaoRespostaId = 2;
            var ordem = 3;

            var entidade = new QuestaoOpcaoResposta(questaoId, opcaoRespostaId, ordem);

            Assert.Equal(questaoId, entidade.QuestaoId);
            Assert.Equal(opcaoRespostaId, entidade.OpcaoRespostaId);
            Assert.Equal(ordem, entidade.Ordem);
        }

        [Fact]
        public void Deve_possuir_navegacoes_para_ef_core()
        {
            var entidade = CriarEntidadePadrao();

            Assert.Null(entidade.Questao);
            Assert.Null(entidade.OpcaoResposta);
        }

        private static QuestaoOpcaoResposta CriarEntidadePadrao()
        {
            return new QuestaoOpcaoResposta(
                questaoId: 10,
                opcaoRespostaId: 20,
                ordem: 1
            );
        }
    }
}
