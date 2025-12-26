using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class CicloTeste
    {
        [Fact]
        public void Construtor_DeveAtribuirValoresCorretamente()
        {
            var codCicloEnsinoEol = 1;
            var descCiclo = "Ensino Fundamental";

            var ciclo = new Ciclo(codCicloEnsinoEol, descCiclo);

            Assert.Equal(codCicloEnsinoEol, ciclo.CodCicloEnsinoEol);
            Assert.Equal(descCiclo, ciclo.DescCiclo);
        }

        [Fact]
        public void Construtor_DeveInicializarColecaoDeQuestionarios()
        {
            var ciclo = new Ciclo(1, "Ensino Médio");

            Assert.NotNull(ciclo.Questionarios);
            Assert.Empty(ciclo.Questionarios);
        }

        [Fact]
        public void DescCiclo_NaoDeveSerNulo()
        {
            var ciclo = new Ciclo(2, "Educação Infantil");

            Assert.False(string.IsNullOrEmpty(ciclo.DescCiclo));
        }
    }
}
