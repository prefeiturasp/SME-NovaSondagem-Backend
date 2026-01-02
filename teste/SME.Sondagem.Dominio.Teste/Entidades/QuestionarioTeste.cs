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
            var modalidadeDesc = "Regular";
            int? dreId = 10;
            var dreNome = "DRE Centro";
            int? ueId = 20;
            var ueNome = "EMEF Teste";
            int? serieAno = 3;
            var serieAnoNome = "3º Ano";
            int? turmaId = 100;
            var turmaNome = "3A";
            var componenteCurricularId = 5;
            var proficienciaId = 7;
            var cicloId = 2;

            var questionario = new Questionario
            {
                Nome = nome,
                Tipo = tipo,
                AnoLetivo = anoLetivo,
                ModalidadeId = modalidadeId,
                ModalidadeDesc = modalidadeDesc,
                DreId = dreId,
                DreNome = dreNome,
                UeId = ueId,
                UeNome = ueNome,
                SerieAno = serieAno,
                SerieAnoNome = serieAnoNome,
                TurmaId = turmaId,
                TurmaNome = turmaNome,
                ComponenteCurricularId = componenteCurricularId,
                ProficienciaId = proficienciaId,
                CicloId = cicloId
            };

            Assert.Equal(nome, questionario.Nome);
            Assert.Equal(tipo, questionario.Tipo);
            Assert.Equal(anoLetivo, questionario.AnoLetivo);
            Assert.Equal(modalidadeId, questionario.ModalidadeId);
            Assert.Equal(modalidadeDesc, questionario.ModalidadeDesc);
            Assert.Equal(dreId, questionario.DreId);
            Assert.Equal(dreNome, questionario.DreNome);
            Assert.Equal(ueId, questionario.UeId);
            Assert.Equal(ueNome, questionario.UeNome);
            Assert.Equal(serieAno, questionario.SerieAno);
            Assert.Equal(serieAnoNome, questionario.SerieAnoNome);
            Assert.Equal(turmaId, questionario.TurmaId);
            Assert.Equal(turmaNome, questionario.TurmaNome);
            Assert.Equal(componenteCurricularId, questionario.ComponenteCurricularId);
            Assert.Equal(proficienciaId, questionario.ProficienciaId);
            Assert.Equal(cicloId, questionario.CicloId);
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
            Assert.Null(questionario.Ciclo);
        }

        private static Questionario CriarQuestionarioPadrao()
        {
            return new Questionario
            {
                Nome = "Teste",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                CicloId = 1
            };
        }
    }
}
