using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class ComponenteCurricularTeste
    {
        [Fact]
        public void Construtor_DeveAtribuirValoresCorretamente()
        {
            var nome = "Matemática";
            int? ano = 5;
            var modalidade = "Ensino Fundamental";
            var codigoEol = 123;

            var componente = new ComponenteCurricular(nome, ano, modalidade, codigoEol);

            Assert.Equal(nome, componente.Nome);
            Assert.Equal(ano, componente.Ano);
            Assert.Equal(modalidade, componente.Modalidade);
            Assert.Equal(codigoEol, componente.CodigoEol);
        }

        [Fact]
        public void Construtor_DeveInicializarColecaoDeProficiencias()
        {
            var componente = new ComponenteCurricular("História", 8, "EF", 789);

            Assert.NotNull(componente.Proficiencias);
            Assert.Empty(componente.Proficiencias);
        }

        [Fact]
        public void Construtor_DeveInicializarColecaoDeQuestionarios()
        {
            var componente = new ComponenteCurricular("Geografia", 6, "EF", 321);

            Assert.NotNull(componente.Questionarios);
            Assert.Empty(componente.Questionarios);
        }

        [Theory]
        [InlineData("Ciências")]
        [InlineData("Língua Portuguesa")]
        public void Nome_NaoDeveSerNuloOuVazio(string nome)
        {
            var componente = new ComponenteCurricular(nome, 7, "Fundamental", 654);

            Assert.False(string.IsNullOrWhiteSpace(componente.Nome));
        }
    }
}
