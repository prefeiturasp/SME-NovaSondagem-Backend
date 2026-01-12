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
            var sondagemId = 10;

            var questionario = new Questionario
            {
                Nome = nome,
                Tipo = tipo,
                AnoLetivo = anoLetivo,
                ModalidadeId = modalidadeId,
                SerieAno = serieAno,
                ComponenteCurricularId = componenteCurricularId,
                ProficienciaId = proficienciaId,
                SondagemId = sondagemId
            };

            Assert.Equal(nome, questionario.Nome);
            Assert.Equal(tipo, questionario.Tipo);
            Assert.Equal(anoLetivo, questionario.AnoLetivo);
            Assert.Equal(modalidadeId, questionario.ModalidadeId);
            Assert.Equal(serieAno, questionario.SerieAno);
            Assert.Equal(componenteCurricularId, questionario.ComponenteCurricularId);
            Assert.Equal(proficienciaId, questionario.ProficienciaId);
            Assert.Equal(sondagemId, questionario.SondagemId);
        }

        [Fact]
        public void Deve_criar_questionario_com_propriedades_obrigatorias()
        {
            var nome = "Questionário Matemática";
            var tipo = TipoQuestionario.SondagemEscrita;
            var anoLetivo = 2024;
            var componenteCurricularId = 1;
            var proficienciaId = 2;
            var sondagemId = 3;

            var questionario = new Questionario
            {
                Nome = nome,
                Tipo = tipo,
                AnoLetivo = anoLetivo,
                ComponenteCurricularId = componenteCurricularId,
                ProficienciaId = proficienciaId,
                SondagemId = sondagemId
            };

            Assert.Equal(nome, questionario.Nome);
            Assert.Equal(tipo, questionario.Tipo);
            Assert.Equal(anoLetivo, questionario.AnoLetivo);
            Assert.Equal(componenteCurricularId, questionario.ComponenteCurricularId);
            Assert.Equal(proficienciaId, questionario.ProficienciaId);
            Assert.Equal(sondagemId, questionario.SondagemId);
        }

        [Fact]
        public void Deve_criar_questionario_com_propriedades_opcionais_nulas()
        {
            var questionario = CriarQuestionarioPadrao();

            Assert.Null(questionario.ModalidadeId);
            Assert.Null(questionario.SerieAno);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_questoes_vazia()
        {
            var questionario = CriarQuestionarioPadrao();

            Assert.NotNull(questionario.Questoes);
            Assert.Empty(questionario.Questoes);
        }

        [Fact]
        public void Deve_possuir_navegacoes_para_ef_core()
        {
            var questionario = CriarQuestionarioPadrao();

            Assert.Null(questionario.Sondagem);
            Assert.Null(questionario.ComponenteCurricular);
            Assert.Null(questionario.Proficiencia);
        }

        [Fact]
        public void Deve_aceitar_modalidade_id_nulo()
        {
            var questionario = new Questionario
            {
                Nome = "Teste",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                ModalidadeId = null
            };

            Assert.Null(questionario.ModalidadeId);
        }

        [Fact]
        public void Deve_aceitar_serie_ano_nulo()
        {
            var questionario = new Questionario
            {
                Nome = "Teste",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                SerieAno = null
            };

            Assert.Null(questionario.SerieAno);
        }

        [Fact]
        public void Deve_aceitar_modalidade_id_com_valor()
        {
            var modalidadeId = 5;
            var questionario = new Questionario
            {
                Nome = "Teste",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                ModalidadeId = modalidadeId
            };

            Assert.Equal(modalidadeId, questionario.ModalidadeId);
        }

        [Fact]
        public void Deve_aceitar_serie_ano_com_valor()
        {
            var serieAno = 7;
            var questionario = new Questionario
            {
                Nome = "Teste",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                SerieAno = serieAno
            };

            Assert.Equal(serieAno, questionario.SerieAno);
        }

        [Fact]
        public void Deve_aceitar_diferentes_tipos_de_questionario()
        {
            var tipoAvaliacao = TipoQuestionario.RegistroAcaoBuscaAtiva;
            var questionario = new Questionario
            {
                Nome = "Teste",
                Tipo = tipoAvaliacao,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1
            };

            Assert.Equal(tipoAvaliacao, questionario.Tipo);
        }

        [Fact]
        public void Deve_aceitar_diferentes_anos_letivos()
        {
            var anoLetivo = 2026;
            var questionario = new Questionario
            {
                Nome = "Teste",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = anoLetivo,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1
            };

            Assert.Equal(anoLetivo, questionario.AnoLetivo);
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
                SondagemId = 1
            };
        }
    }
}
