using SME.Sondagem.Dominio.Enums;
using Xunit;
using Proficiencia = SME.Sondagem.Dominio.Entidades.Proficiencia;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class ProficienciaTeste
    {
        [Fact]
        public void Deve_criar_proficiencia_com_dados_validos()
        {
            var nome = "Leitura";
            var componenteCurricularId = 10;
            var modalidade = (int)Modalidade.Fundamental;

            var proficiencia = new Proficiencia(nome, componenteCurricularId,modalidade);

            Assert.Equal(nome, proficiencia.Nome);
            Assert.Equal(componenteCurricularId, proficiencia.ComponenteCurricularId);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_questionarios()
        {
            var modalidade = (int)Modalidade.Fundamental;
            var proficiencia = new Proficiencia("Escrita", 5,modalidade);

            Assert.NotNull(proficiencia.Questionarios);
            Assert.Empty(proficiencia.Questionarios);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_componente_curricular()
        {
            var modalidade = (int)Modalidade.Fundamental;
            var proficiencia = new Proficiencia("Matemática", 3,modalidade);

            Assert.NotNull(proficiencia);
            Assert.Null(proficiencia.ComponenteCurricular);
        }
    }
}
