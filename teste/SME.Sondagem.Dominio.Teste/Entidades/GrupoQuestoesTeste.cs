using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class GrupoQuestoesTeste
    {
        [Fact]
        public void Deve_criar_grupo_questoes_com_titulo_e_subtitulo()
        {
            var titulo = "Leitura e Escrita";
            var subtitulo = "Avaliação diagnóstica";

            var grupo = new GrupoQuestoes(titulo, subtitulo);

            Assert.Equal(titulo, grupo.Titulo);
            Assert.Equal(subtitulo, grupo.Subtitulo);
        }

        [Fact]
        public void Deve_criar_grupo_questoes_com_subtitulo_nulo()
        {
            // Arrange
            var titulo = "Matemática";

            var grupo = new GrupoQuestoes(titulo, null);

            Assert.Equal(titulo, grupo.Titulo);
            Assert.Null(grupo.Subtitulo);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_questoes_vazia()
        {
            var grupo = CriarGrupoPadrao();

            Assert.NotNull(grupo.Questoes);
            Assert.Empty(grupo.Questoes);
        }

        private static GrupoQuestoes CriarGrupoPadrao()
        {
            return new GrupoQuestoes(
                titulo: "Grupo Padrão",
                subtitulo: null
            );
        }
    }
}
