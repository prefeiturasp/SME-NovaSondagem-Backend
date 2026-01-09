using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class BimestreTeste
    {
        [Fact]
        public void Construtor_DeveAtribuirValoresCorretamente()
        {
            var codBimestreEnsinoEol = 1;
            var descricao = "Inicial";

            var bimestre = new Bimestre(codBimestreEnsinoEol, descricao);

            Assert.Equal(codBimestreEnsinoEol, bimestre.CodBimestreEnsinoEol);
            Assert.Equal(descricao, bimestre.Descricao);
        }

        [Fact]
        public void Construtor_DeveInicializarColecaoDeQuestionarios()
        {
            var bimestre = new Bimestre(1, "Primeiro");

            Assert.NotNull(bimestre.Questionarios);
            Assert.Empty(bimestre.Questionarios);
        }

        [Fact]
        public void Descricao_NaoDeveSerNulo()
        {
            var bimestre = new Bimestre(2, "Segundo");

            Assert.False(string.IsNullOrEmpty(bimestre.Descricao));
        }
    }
}
