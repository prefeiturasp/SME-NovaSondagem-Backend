using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class QuestionarioBimestreTeste
    {
        [Fact]
        public void Deve_Criar_QuestionarioBimestre_Com_Dados_Validos()
        {
            var questionarioId = 10;
            var bimestreId = 2;

            var questionarioBimestre = new QuestionarioBimestre(questionarioId, bimestreId);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_QuestionarioId_For_Zero()
        {
            var questionarioId = 0;
            var bimestreId = 1;

            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(questionarioId, bimestreId));

            Assert.Equal("questionarioId", exception.ParamName);
            Assert.Contains("Questionário ID deve ser maior que zero", exception.Message);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_QuestionarioId_For_Negativo()
        {
            var questionarioId = -1;
            var bimestreId = 1;

            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(questionarioId, bimestreId));

            Assert.Equal("questionarioId", exception.ParamName);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_BimestreId_For_Zero()
        {
            var questionarioId = 1;
            var bimestreId = 0;

            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(questionarioId, bimestreId));

            Assert.Equal("bimestreId", exception.ParamName);
            Assert.Contains("Bimestre ID deve ser maior que zero", exception.Message);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_BimestreId_For_Negativo()
        {
            var questionarioId = 1;
            var bimestreId = -1;

            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(questionarioId, bimestreId));

            Assert.Equal("bimestreId", exception.ParamName);
        }

        [Fact]
        public void Deve_Atualizar_QuestionarioBimestre_Com_Novos_Valores()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);
            var novoQuestionarioId = 5;
            var novoBimestreId = 3;

            questionarioBimestre.Atualizar(novoQuestionarioId, novoBimestreId);

            Assert.Equal(novoQuestionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(novoBimestreId, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Ao_Atualizar_Com_QuestionarioId_Invalido()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            var exception = Assert.Throws<ArgumentException>(() =>
                questionarioBimestre.Atualizar(0, 2));

            Assert.Equal("questionarioId", exception.ParamName);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Ao_Atualizar_Com_BimestreId_Invalido()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            var exception = Assert.Throws<ArgumentException>(() =>
                questionarioBimestre.Atualizar(2, 0));

            Assert.Equal("bimestreId", exception.ParamName);
        }

        [Fact]
        public void Deve_Manter_QuestionarioId_Quando_Atualizar_Apenas_BimestreId()
        {
            var questionarioIdOriginal = 10;
            var questionarioBimestre = new QuestionarioBimestre(questionarioIdOriginal, 1);

            questionarioBimestre.Atualizar(questionarioIdOriginal, 3);

            Assert.Equal(questionarioIdOriginal, questionarioBimestre.QuestionarioId);
            Assert.Equal(3, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Deve_Manter_BimestreId_Quando_Atualizar_Apenas_QuestionarioId()
        {
            var bimestreIdOriginal = 2;
            var questionarioBimestre = new QuestionarioBimestre(1, bimestreIdOriginal);

            questionarioBimestre.Atualizar(5, bimestreIdOriginal);

            Assert.Equal(5, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreIdOriginal, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Deve_Possuir_Propriedades_De_Navegacao_Para_EF_Core()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            Assert.Null(questionarioBimestre.Questionario);
            Assert.Null(questionarioBimestre.Bimestre);
        }

        [Fact]
        public void Deve_Aceitar_Valores_Maximos_Para_QuestionarioId_E_BimestreId()
        {
            var questionarioId = int.MaxValue;
            var bimestreId = int.MaxValue;

            var questionarioBimestre = new QuestionarioBimestre(questionarioId, bimestreId);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Deve_Aceitar_Valor_1_Para_QuestionarioId_E_BimestreId()
        {
            var questionarioId = 1;
            var bimestreId = 1;

            var questionarioBimestre = new QuestionarioBimestre(questionarioId, bimestreId);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }
    }
}