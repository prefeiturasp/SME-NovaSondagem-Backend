using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class QuestionarioTeste
    {
        [Fact]
        public void Deve_criar_questionario_com_todos_os_dados()
        {
            var nome = "Questionário Diagnóstico";
            var tipo = TipoQuestionario.SondagemEscrita;
            var anoLetivo = 2025;
            int? modalidadeId = 1;
            int? serieAno = 3;
            var componenteCurricularId = 5;
            var proficienciaId = 7;

            var questionario = new Questionario
            {
                Nome = nome,
                Tipo = tipo,
                AnoLetivo = anoLetivo,
                ModalidadeId = modalidadeId,
                SerieAno = serieAno,
                ComponenteCurricularId = componenteCurricularId,
                ProficienciaId = proficienciaId,
            };

            Assert.Equal(nome, questionario.Nome);
            Assert.Equal(tipo, questionario.Tipo);
            Assert.Equal(anoLetivo, questionario.AnoLetivo);
            Assert.Equal(modalidadeId, questionario.ModalidadeId);
            Assert.Equal(serieAno, questionario.SerieAno);
            Assert.Equal(componenteCurricularId, questionario.ComponenteCurricularId);
            Assert.Equal(proficienciaId, questionario.ProficienciaId);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_questoes_vazia()
        {
            var questionario = CriarQuestionarioPadrao();

            Assert.NotNull(questionario.Questoes);
            Assert.Empty(questionario.Questoes);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_sondagens_vazia()
        {
            var questionario = CriarQuestionarioPadrao();

            Assert.NotNull(questionario.Sondagens);
            Assert.Empty(questionario.Sondagens);
        }

        [Fact]
        public void Deve_possuir_navegacoes_para_ef_core()
        {
            var questionario = CriarQuestionarioPadrao();

            // Navegações são null até serem carregadas pelo EF
            Assert.Null(questionario.ComponenteCurricular);
            Assert.Null(questionario.Proficiencia);
        }

        private static Questionario CriarQuestionarioPadrao()
        {
            return new Questionario
            {
                Nome = "Teste",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1
            };
        }
    }
}
