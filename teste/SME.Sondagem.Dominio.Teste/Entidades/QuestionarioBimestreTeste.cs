using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class QuestionarioBimestreTeste
    {
        #region Construtor

        [Fact]
        public void Construtor_DeveCriarQuestionarioBimestreComDadosValidos()
        {
            var questionarioId = 10;
            var bimestreId = 2;

            var questionarioBimestre = new QuestionarioBimestre(questionarioId, bimestreId);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(10, 3)]
        [InlineData(100, 4)]
        [InlineData(999, 1)]
        public void Construtor_DeveAceitarDiferentesCombinacoesValidas(int questionarioId, int bimestreId)
        {
            var questionarioBimestre = new QuestionarioBimestre(questionarioId, bimestreId);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(-10, 1)]
        [InlineData(-999, 1)]
        public void Construtor_DeveLancarExcecaoQuandoQuestionarioIdForMenorOuIgualAZero(int questionarioIdInvalido, int bimestreId)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(questionarioIdInvalido, bimestreId));

            Assert.Equal("questionarioId", exception.ParamName);
            Assert.Contains("Questionário ID deve ser maior que zero", exception.Message);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        [InlineData(1, -10)]
        [InlineData(1, -999)]
        public void Construtor_DeveLancarExcecaoQuandoBimestreIdForMenorOuIgualAZero(int questionarioId, int bimestreIdInvalido)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(questionarioId, bimestreIdInvalido));

            Assert.Equal("bimestreId", exception.ParamName);
            Assert.Contains("Bimestre ID deve ser maior que zero", exception.Message);
        }

        [Fact]
        public void Construtor_DeveLancarExcecaoQuandoAmbosOsIdsForemInvalidos()
        {
            var questionarioIdInvalido = 0;
            var bimestreIdInvalido = 0;

            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(questionarioIdInvalido, bimestreIdInvalido));

            Assert.Equal("questionarioId", exception.ParamName);
        }

        [Fact]
        public void Construtor_DeveAceitarValorMinimoValido()
        {
            var questionarioId = 1;
            var bimestreId = 1;

            var questionarioBimestre = new QuestionarioBimestre(questionarioId, bimestreId);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Construtor_DeveAceitarValoresMaximos()
        {
            var questionarioId = int.MaxValue;
            var bimestreId = int.MaxValue;

            var questionarioBimestre = new QuestionarioBimestre(questionarioId, bimestreId);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }

        #endregion

        #region Atualizar

        [Fact]
        public void Atualizar_DeveAtualizarQuestionarioBimestreComNovosValores()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);
            var novoQuestionarioId = 5;
            var novoBimestreId = 3;

            questionarioBimestre.Atualizar(novoQuestionarioId, novoBimestreId);

            Assert.Equal(novoQuestionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(novoBimestreId, questionarioBimestre.BimestreId);
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        [InlineData(1, 1, 10, 4)]
        [InlineData(5, 2, 15, 3)]
        [InlineData(10, 4, 20, 1)]
        public void Atualizar_DeveAceitarDiferentesCombinacoes(
            int questionarioIdOriginal,
            int bimestreIdOriginal,
            int novoQuestionarioId,
            int novoBimestreId)
        {
            var questionarioBimestre = new QuestionarioBimestre(questionarioIdOriginal, bimestreIdOriginal);

            questionarioBimestre.Atualizar(novoQuestionarioId, novoBimestreId);

            Assert.Equal(novoQuestionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(novoBimestreId, questionarioBimestre.BimestreId);
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(-1, 2)]
        [InlineData(-10, 2)]
        public void Atualizar_DeveLancarExcecaoQuandoQuestionarioIdForInvalido(int questionarioIdInvalido, int bimestreId)
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            var exception = Assert.Throws<ArgumentException>(() =>
                questionarioBimestre.Atualizar(questionarioIdInvalido, bimestreId));

            Assert.Equal("questionarioId", exception.ParamName);
            Assert.Contains("Questionário ID deve ser maior que zero", exception.Message);
        }

        [Theory]
        [InlineData(2, 0)]
        [InlineData(2, -1)]
        [InlineData(2, -10)]
        public void Atualizar_DeveLancarExcecaoQuandoBimestreIdForInvalido(int questionarioId, int bimestreIdInvalido)
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            var exception = Assert.Throws<ArgumentException>(() =>
                questionarioBimestre.Atualizar(questionarioId, bimestreIdInvalido));

            Assert.Equal("bimestreId", exception.ParamName);
            Assert.Contains("Bimestre ID deve ser maior que zero", exception.Message);
        }

        [Fact]
        public void Atualizar_DeveManterQuestionarioIdQuandoAtualizarApenasBimestreId()
        {
            var questionarioIdOriginal = 10;
            var questionarioBimestre = new QuestionarioBimestre(questionarioIdOriginal, 1);

            questionarioBimestre.Atualizar(questionarioIdOriginal, 3);

            Assert.Equal(questionarioIdOriginal, questionarioBimestre.QuestionarioId);
            Assert.Equal(3, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Atualizar_DeveManterBimestreIdQuandoAtualizarApenasQuestionarioId()
        {
            var bimestreIdOriginal = 2;
            var questionarioBimestre = new QuestionarioBimestre(1, bimestreIdOriginal);

            questionarioBimestre.Atualizar(5, bimestreIdOriginal);

            Assert.Equal(5, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreIdOriginal, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Atualizar_DeveManterValoresOriginaisQuandoQuestionarioIdForInvalido()
        {
            var questionarioIdOriginal = 10;
            var bimestreIdOriginal = 2;
            var questionarioBimestre = new QuestionarioBimestre(questionarioIdOriginal, bimestreIdOriginal);

            Assert.Throws<ArgumentException>(() =>
                questionarioBimestre.Atualizar(0, 3));

            Assert.Equal(questionarioIdOriginal, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreIdOriginal, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Atualizar_DeveManterValoresOriginaisQuandoBimestreIdForInvalido()
        {
            var questionarioIdOriginal = 10;
            var bimestreIdOriginal = 2;
            var questionarioBimestre = new QuestionarioBimestre(questionarioIdOriginal, bimestreIdOriginal);

            Assert.Throws<ArgumentException>(() =>
                questionarioBimestre.Atualizar(5, 0));

            Assert.Equal(questionarioIdOriginal, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreIdOriginal, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void Atualizar_DevePermitirAtualizarParaMesmosValores()
        {
            var questionarioId = 5;
            var bimestreId = 3;
            var questionarioBimestre = new QuestionarioBimestre(questionarioId, bimestreId);

            questionarioBimestre.Atualizar(questionarioId, bimestreId);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }

        #endregion

        #region Propriedades de Navegação

        [Fact]
        public void PropriedadesDeNavegacao_DevemSerNulasInicialmente()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            Assert.Null(questionarioBimestre.Questionario);
            Assert.Null(questionarioBimestre.Bimestre);
        }

        [Fact]
        public void Questionario_DeveSerPropriedadeDeNavegacao()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            Assert.IsType<QuestionarioBimestre>(questionarioBimestre);
            var propriedade = questionarioBimestre.GetType().GetProperty("Questionario");
            Assert.NotNull(propriedade);
            Assert.True(propriedade.PropertyType == typeof(Questionario));
        }

        [Fact]
        public void Bimestre_DeveSerPropriedadeDeNavegacao()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            Assert.IsType<QuestionarioBimestre>(questionarioBimestre);
            var propriedade = questionarioBimestre.GetType().GetProperty("Bimestre");
            Assert.NotNull(propriedade);
            Assert.True(propriedade.PropertyType == typeof(Bimestre));
        }

        #endregion

        #region Validações Gerais

        [Fact]
        public void QuestionarioBimestre_DeveHerdarDeEntidadeBase()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            Assert.IsType<EntidadeBase>(questionarioBimestre, exactMatch: false);
        }

        [Fact]
        public void QuestionarioId_DeveSerSemprePositivoAposCriacao()
        {
            var questionarioBimestre = new QuestionarioBimestre(5, 2);

            Assert.True(questionarioBimestre.QuestionarioId > 0);
        }

        [Fact]
        public void BimestreId_DeveSerSemprePositivoAposCriacao()
        {
            var questionarioBimestre = new QuestionarioBimestre(5, 2);

            Assert.True(questionarioBimestre.BimestreId > 0);
        }

        #endregion

        #region Cenários de Integração

        [Fact]
        public void MultiplasAtualizacoes_DevemManterConsistencia()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            questionarioBimestre.Atualizar(2, 2);
            questionarioBimestre.Atualizar(3, 3);
            questionarioBimestre.Atualizar(4, 4);

            Assert.Equal(4, questionarioBimestre.QuestionarioId);
            Assert.Equal(4, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void AtualizacoesAlternadas_DevemFuncionarCorretamente()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            questionarioBimestre.Atualizar(2, 1);
            Assert.Equal(2, questionarioBimestre.QuestionarioId);
            Assert.Equal(1, questionarioBimestre.BimestreId);

            questionarioBimestre.Atualizar(2, 3);
            Assert.Equal(2, questionarioBimestre.QuestionarioId);
            Assert.Equal(3, questionarioBimestre.BimestreId);

            questionarioBimestre.Atualizar(5, 3);
            Assert.Equal(5, questionarioBimestre.QuestionarioId);
            Assert.Equal(3, questionarioBimestre.BimestreId);
        }

        [Fact]
        public void AtualizacoesSequenciais_DevemPermitirVoltarParaValoresAnteriores()
        {
            var questionarioIdOriginal = 1;
            var bimestreIdOriginal = 1;
            var questionarioBimestre = new QuestionarioBimestre(questionarioIdOriginal, bimestreIdOriginal);

            questionarioBimestre.Atualizar(2, 2);
            questionarioBimestre.Atualizar(3, 3);
            questionarioBimestre.Atualizar(questionarioIdOriginal, bimestreIdOriginal);

            Assert.Equal(questionarioIdOriginal, questionarioBimestre.QuestionarioId);
            Assert.Equal(bimestreIdOriginal, questionarioBimestre.BimestreId);
        }

        #endregion

        #region Testes de Limites

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(int.MaxValue)]
        public void Construtor_DeveAceitarDiferentesValoresValidosParaQuestionarioId(int questionarioId)
        {
            var questionarioBimestre = new QuestionarioBimestre(questionarioId, 1);

            Assert.Equal(questionarioId, questionarioBimestre.QuestionarioId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void Construtor_DeveAceitarDiferentesValoresValidosParaBimestreId(int bimestreId)
        {
            var questionarioBimestre = new QuestionarioBimestre(1, bimestreId);

            Assert.Equal(bimestreId, questionarioBimestre.BimestreId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void Atualizar_DeveAceitarDiferentesValoresValidosParaQuestionarioId(int novoQuestionarioId)
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            questionarioBimestre.Atualizar(novoQuestionarioId, 1);

            Assert.Equal(novoQuestionarioId, questionarioBimestre.QuestionarioId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(int.MaxValue)]
        public void Atualizar_DeveAceitarDiferentesValoresValidosParaBimestreId(int novoBimestreId)
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            questionarioBimestre.Atualizar(1, novoBimestreId);

            Assert.Equal(novoBimestreId, questionarioBimestre.BimestreId);
        }

        #endregion

        #region Testes de Mensagens de Erro

        [Fact]
        public void Construtor_MensagemDeErro_DeveSerEspecificaParaQuestionarioId()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(0, 1));

            Assert.Equal("Questionário ID deve ser maior que zero. (Parameter 'questionarioId')", exception.Message);
        }

        [Fact]
        public void Construtor_MensagemDeErro_DeveSerEspecificaParaBimestreId()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new QuestionarioBimestre(1, 0));

            Assert.Equal("Bimestre ID deve ser maior que zero. (Parameter 'bimestreId')", exception.Message);
        }

        [Fact]
        public void Atualizar_MensagemDeErro_DeveSerEspecificaParaQuestionarioId()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            var exception = Assert.Throws<ArgumentException>(() =>
                questionarioBimestre.Atualizar(0, 1));

            Assert.Equal("Questionário ID deve ser maior que zero. (Parameter 'questionarioId')", exception.Message);
        }

        [Fact]
        public void Atualizar_MensagemDeErro_DeveSerEspecificaParaBimestreId()
        {
            var questionarioBimestre = new QuestionarioBimestre(1, 1);

            var exception = Assert.Throws<ArgumentException>(() =>
                questionarioBimestre.Atualizar(1, 0));

            Assert.Equal("Bimestre ID deve ser maior que zero. (Parameter 'bimestreId')", exception.Message);
        }

        #endregion
    }
}