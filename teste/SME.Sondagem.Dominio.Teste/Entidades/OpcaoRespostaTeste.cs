using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class OpcaoRespostaTeste
    {
        [Fact]
        public void Deve_criar_opcao_resposta_com_todos_os_dados()
        {
            var descricao = "Sim";
            var legenda = "Resposta positiva";
            var corFundo = "#00FF00";
            var corTexto = "#000000";

            var opcao = new OpcaoResposta(descricao, legenda, corFundo, corTexto);

            Assert.Equal(descricao, opcao.DescricaoOpcaoResposta);
            Assert.Equal(legenda, opcao.Legenda);
            Assert.Equal(corFundo, opcao.CorFundo);
            Assert.Equal(corTexto, opcao.CorTexto);
        }

        [Fact]
        public void Deve_criar_opcao_resposta_com_campos_opcionais_nulos()
        {
            var descricao = "Não";

            var opcao = new OpcaoResposta(descricao, null, null, null);

            Assert.Equal(descricao, opcao.DescricaoOpcaoResposta);
            Assert.Null(opcao.Legenda);
            Assert.Null(opcao.CorFundo);
            Assert.Null(opcao.CorTexto);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_questao_opcoes_vazia()
        {
            var opcao = CriarOpcaoPadrao();

            Assert.NotNull(opcao.QuestaoOpcoes);
            Assert.Empty(opcao.QuestaoOpcoes);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_respostas_vazia()
        {
            var opcao = CriarOpcaoPadrao();

            Assert.NotNull(opcao.Respostas);
            Assert.Empty(opcao.Respostas);
        }

        private static OpcaoResposta CriarOpcaoPadrao()
        {
            return new OpcaoResposta(
                descricaoOpcaoResposta: "Padrão",
                legenda: null,
                corFundo: null,
                corTexto: null
            );
        }
    }
}
