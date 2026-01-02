using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class ProficienciaTeste
    {
        [Fact]
        public void Deve_criar_proficiencia_com_dados_validos()
        {
            var nome = "Leitura";
            var componenteCurricularId = 10;

            var proficiencia = new Proficiencia(nome, componenteCurricularId);

            Assert.Equal(nome, proficiencia.Nome);
            Assert.Equal(componenteCurricularId, proficiencia.ComponenteCurricularId);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_questionarios()
        {
            var proficiencia = new Proficiencia("Escrita", 5);

            Assert.NotNull(proficiencia.Questionarios);
            Assert.Empty(proficiencia.Questionarios);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_componente_curricular()
        {
            var proficiencia = new Proficiencia("Matemática", 3);

            Assert.NotNull(proficiencia);
            Assert.Null(proficiencia.ComponenteCurricular);
        }
    }
}
