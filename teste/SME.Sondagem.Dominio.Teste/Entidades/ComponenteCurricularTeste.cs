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
            string? modalidade = "Ensino Fundamental";
            var codigoEol = 123;

            var componente = new ComponenteCurricular(nome, ano, modalidade, codigoEol);

            Assert.Equal(nome, componente.Nome);
            Assert.Equal(ano, componente.Ano);
            Assert.Equal(modalidade, componente.Modalidade);
            Assert.Equal(codigoEol, componente.CodigoEol);
        }

        [Fact]
        public void Construtor_DevePermitirValoresOpcionaisNulos()
        {
            var componente = new ComponenteCurricular(
                nome: "Português",
                ano: null,
                modalidade: null,
                codigoEol: 456);

            Assert.Equal("Português", componente.Nome);
            Assert.Null(componente.Ano);
            Assert.Null(componente.Modalidade);
            Assert.Equal(456, componente.CodigoEol);
        }

        [Fact]
        public void Construtor_DeveInicializarColecaoDeProficiencias()
        {
            var componente = new ComponenteCurricular("História", 8, null, 789);

            Assert.NotNull(componente.Proficiencias);
            Assert.Empty(componente.Proficiencias);
        }

        [Fact]
        public void Construtor_DeveInicializarColecaoDeQuestionarios()
        {
            var componente = new ComponenteCurricular("Geografia", 6, null, 321);

            Assert.NotNull(componente.Questionarios);
            Assert.Empty(componente.Questionarios);
        }

        [Fact]
        public void Nome_NaoDeveSerNuloOuVazio()
        {
            var componente = new ComponenteCurricular("Ciências", 7, "Fundamental", 654);

            Assert.False(string.IsNullOrWhiteSpace(componente.Nome));
        }
    }
}
