using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class BimestreTeste
    {
        #region Construtor

        [Fact]
        public void Construtor_DeveAtribuirValoresCorretamente()
        {
            var codBimestreEnsinoEol = 1;
            var descricao = "Primeiro Bimestre";

            var bimestre = new Bimestre(codBimestreEnsinoEol, descricao);

            Assert.Equal(codBimestreEnsinoEol, bimestre.CodBimestreEnsinoEol);
            Assert.Equal(descricao, bimestre.Descricao);
        }

        [Theory]
        [InlineData(1, "Primeiro Bimestre")]
        [InlineData(2, "Segundo Bimestre")]
        [InlineData(3, "Terceiro Bimestre")]
        [InlineData(4, "Quarto Bimestre")]
        public void Construtor_DeveAceitarDiferentesValoresValidos(int codigo, string descricao)
        {
            var bimestre = new Bimestre(codigo, descricao);

            Assert.Equal(codigo, bimestre.CodBimestreEnsinoEol);
            Assert.Equal(descricao, bimestre.Descricao);
        }

        [Fact]
        public void Construtor_DeveInicializarColecoesDeNavegacao()
        {
            var bimestre = new Bimestre(1, "Primeiro Bimestre");

            Assert.NotNull(bimestre.PeriodosBimestre);
            Assert.NotNull(bimestre.RespostaAlunos);
            Assert.NotNull(bimestre.QuestionariosBimestres);
            Assert.Empty(bimestre.PeriodosBimestre);
            Assert.Empty(bimestre.RespostaAlunos);
            Assert.Empty(bimestre.QuestionariosBimestres);
        }

        #endregion

        #region AtualizarDescricao

        [Fact]
        public void AtualizarDescricao_DeveAtualizarDescricaoCorretamente()
        {
            var bimestre = new Bimestre(1, "Descrição Inicial");
            var novaDescricao = "Descrição Atualizada";

            bimestre.AtualizarDescricao(novaDescricao);

            Assert.Equal(novaDescricao, bimestre.Descricao);
        }

        [Theory]
        [InlineData("Primeiro Bimestre")]
        [InlineData("Segundo Bimestre - 2024")]
        [InlineData("Bimestre Inicial")]
        public void AtualizarDescricao_DeveAceitarDescricoesValidas(string novaDescricao)
        {
            var bimestre = new Bimestre(1, "Descrição Original");

            bimestre.AtualizarDescricao(novaDescricao);

            Assert.Equal(novaDescricao, bimestre.Descricao);
        }

        [Fact]
        public void AtualizarDescricao_DeveLancarExcecao_QuandoDescricaoForVazia()
        {
            var bimestre = new Bimestre(1, "Descrição Inicial");

            var exception = Assert.Throws<ArgumentException>(() =>
                bimestre.AtualizarDescricao(string.Empty));

            Assert.Equal("Descrição do bimestre não pode ser vazia. (Parameter 'descricao')", exception.Message);
            Assert.Equal("descricao", exception.ParamName);
        }

        [Fact]
        public void AtualizarDescricao_DeveLancarExcecao_QuandoDescricaoForNull()
        {
            var bimestre = new Bimestre(1, "Descrição Inicial");

            var exception = Assert.Throws<ArgumentException>(() =>
                bimestre.AtualizarDescricao(null!));

            Assert.Equal("Descrição do bimestre não pode ser vazia. (Parameter 'descricao')", exception.Message);
            Assert.Equal("descricao", exception.ParamName);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("")]
        public void AtualizarDescricao_DeveLancarExcecao_QuandoDescricaoForWhitespace(string descricaoInvalida)
        {
            var bimestre = new Bimestre(1, "Descrição Inicial");

            var exception = Assert.Throws<ArgumentException>(() =>
                bimestre.AtualizarDescricao(descricaoInvalida));

            Assert.Contains("Descrição do bimestre não pode ser vazia", exception.Message);
        }

        [Fact]
        public void AtualizarDescricao_NaoDeveAlterarOutrasPropriedades()
        {
            var codigoOriginal = 1;
            var bimestre = new Bimestre(codigoOriginal, "Descrição Inicial");

            bimestre.AtualizarDescricao("Nova Descrição");

            Assert.Equal(codigoOriginal, bimestre.CodBimestreEnsinoEol);
        }

        #endregion

        #region AtualizarCodigoBimestreEnsino

        [Fact]
        public void AtualizarCodigoBimestreEnsino_DeveAtualizarCodigoCorretamente()
        {
            var bimestre = new Bimestre(1, "Primeiro Bimestre");
            var novoCodigo = 2;

            bimestre.AtualizarCodigoBimestreEnsino(novoCodigo);

            Assert.Equal(novoCodigo, bimestre.CodBimestreEnsinoEol);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(100)]
        [InlineData(999)]
        public void AtualizarCodigoBimestreEnsino_DeveAceitarCodigosValidos(int novoCodigo)
        {
            var bimestre = new Bimestre(1, "Primeiro Bimestre");

            bimestre.AtualizarCodigoBimestreEnsino(novoCodigo);

            Assert.Equal(novoCodigo, bimestre.CodBimestreEnsinoEol);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-999)]
        public void AtualizarCodigoBimestreEnsino_DeveLancarExcecao_QuandoCodigoForMenorOuIgualAZero(int codigoInvalido)
        {
            var bimestre = new Bimestre(1, "Primeiro Bimestre");

            var exception = Assert.Throws<ArgumentException>(() =>
                bimestre.AtualizarCodigoBimestreEnsino(codigoInvalido));

            Assert.Equal("Código do bimestre de ensino deve ser maior que zero. (Parameter 'codBimestreEnsinoEol')", exception.Message);
            Assert.Equal("codBimestreEnsinoEol", exception.ParamName);
        }

        [Fact]
        public void AtualizarCodigoBimestreEnsino_NaoDeveAlterarOutrasPropriedades()
        {
            var descricaoOriginal = "Descrição Original";
            var bimestre = new Bimestre(1, descricaoOriginal);

            bimestre.AtualizarCodigoBimestreEnsino(2);

            Assert.Equal(descricaoOriginal, bimestre.Descricao);
        }

        #endregion

        #region Atualizar

        [Fact]
        public void Atualizar_DeveAtualizarAmbosOsCamposCorretamente()
        {
            var bimestre = new Bimestre(1, "Descrição Inicial");
            var novaDescricao = "Descrição Atualizada";
            var novoCodigo = 2;

            bimestre.Atualizar(novaDescricao, novoCodigo);

            Assert.Equal(novaDescricao, bimestre.Descricao);
            Assert.Equal(novoCodigo, bimestre.CodBimestreEnsinoEol);
        }

        [Theory]
        [InlineData("Primeiro Bimestre", 1)]
        [InlineData("Segundo Bimestre", 2)]
        [InlineData("Terceiro Bimestre", 3)]
        [InlineData("Quarto Bimestre", 4)]
        public void Atualizar_DeveAceitarCombinacoesValidas(string descricao, int codigo)
        {
            var bimestre = new Bimestre(99, "Descrição Antiga");

            bimestre.Atualizar(descricao, codigo);

            Assert.Equal(descricao, bimestre.Descricao);
            Assert.Equal(codigo, bimestre.CodBimestreEnsinoEol);
        }

        [Fact]
        public void Atualizar_DeveLancarExcecao_QuandoDescricaoForInvalida()
        {
            var bimestre = new Bimestre(1, "Descrição Inicial");

            var exception = Assert.Throws<ArgumentException>(() =>
                bimestre.Atualizar(string.Empty, 2));

            Assert.Contains("Descrição do bimestre não pode ser vazia", exception.Message);
        }

        [Fact]
        public void Atualizar_DeveLancarExcecao_QuandoCodigoForInvalido()
        {
            var bimestre = new Bimestre(1, "Descrição Inicial");

            var exception = Assert.Throws<ArgumentException>(() =>
                bimestre.Atualizar("Nova Descrição", 0));

            Assert.Contains("Código do bimestre de ensino deve ser maior que zero", exception.Message);
        }

        [Fact]
        public void Atualizar_DeveLancarExcecao_QuandoAmbosOsValoresForemInvalidos()
        {
            var bimestre = new Bimestre(1, "Descrição Inicial");

            var exception = Assert.Throws<ArgumentException>(() =>
                bimestre.Atualizar(string.Empty, 0));

            Assert.Contains("Descrição do bimestre não pode ser vazia", exception.Message);
        }

        [Fact]
        public void Atualizar_DeveManterValoresOriginais_QuandoDescricaoForInvalida()
        {
            var codigoOriginal = 1;
            var descricaoOriginal = "Descrição Original";
            var bimestre = new Bimestre(codigoOriginal, descricaoOriginal);

            Assert.Throws<ArgumentException>(() =>
                bimestre.Atualizar(string.Empty, 2));

            Assert.Equal(codigoOriginal, bimestre.CodBimestreEnsinoEol);
            Assert.Equal(descricaoOriginal, bimestre.Descricao);
        }

        [Fact]
        public void Atualizar_DeveAtualizarDescricao_MasLancarExcecaoNoCodigo()
        {
            var codigoOriginal = 1;
            var descricaoOriginal = "Descrição Original";
            var novaDescricao = "Nova Descrição";
            var bimestre = new Bimestre(codigoOriginal, descricaoOriginal);

            Assert.Throws<ArgumentException>(() =>
                bimestre.Atualizar(novaDescricao, 0));

            Assert.Equal(novaDescricao, bimestre.Descricao);
        }

        #endregion

        #region Validações Gerais

        [Fact]
        public void Descricao_NaoDeveSerNuloOuVazio()
        {
            var bimestre = new Bimestre(2, "Segundo Bimestre");

            Assert.False(string.IsNullOrEmpty(bimestre.Descricao));
            Assert.False(string.IsNullOrWhiteSpace(bimestre.Descricao));
        }

        [Fact]
        public void CodBimestreEnsinoEol_DeveSerPositivo()
        {
            var bimestre = new Bimestre(5, "Quinto Bimestre");

            Assert.True(bimestre.CodBimestreEnsinoEol > 0);
        }

        [Fact]
        public void Bimestre_DeveHerdarDeEntidadeBase()
        {
            var bimestre = new Bimestre(1, "Primeiro Bimestre");

            Assert.IsAssignableFrom<EntidadeBase>(bimestre);
        }

        #endregion

        #region Cenários de Integração

        [Fact]
        public void MultiplasAtualizacoes_DevemManterConsistencia()
        {
            var bimestre = new Bimestre(1, "Descrição 1");

            bimestre.AtualizarDescricao("Descrição 2");
            bimestre.AtualizarCodigoBimestreEnsino(2);
            bimestre.Atualizar("Descrição 3", 3);

            Assert.Equal("Descrição 3", bimestre.Descricao);
            Assert.Equal(3, bimestre.CodBimestreEnsinoEol);
        }

        [Fact]
        public void AtualizacoesParciais_DevemFuncionarCorretamente()
        {
            var bimestre = new Bimestre(1, "Descrição Original");

            bimestre.AtualizarDescricao("Nova Descrição");

            Assert.Equal("Nova Descrição", bimestre.Descricao);
            Assert.Equal(1, bimestre.CodBimestreEnsinoEol);

            bimestre.AtualizarCodigoBimestreEnsino(2);

            Assert.Equal("Nova Descrição", bimestre.Descricao);
            Assert.Equal(2, bimestre.CodBimestreEnsinoEol);
        }

        #endregion

        #region Testes de Propriedades de Navegação

        [Fact]
        public void PeriodosBimestre_DeveSerInicializadoComoListaVazia()
        {
            var bimestre = new Bimestre(1, "Primeiro Bimestre");

            Assert.NotNull(bimestre.PeriodosBimestre);
            Assert.IsAssignableFrom<ICollection<SondagemPeriodoBimestre>>(bimestre.PeriodosBimestre);
            Assert.Empty(bimestre.PeriodosBimestre);
        }

        [Fact]
        public void RespostaAlunos_DeveSerInicializadoComoListaVazia()
        {
            var bimestre = new Bimestre(1, "Primeiro Bimestre");

            Assert.NotNull(bimestre.RespostaAlunos);
            Assert.IsAssignableFrom<ICollection<RespostaAluno>>(bimestre.RespostaAlunos);
            Assert.Empty(bimestre.RespostaAlunos);
        }

        [Fact]
        public void QuestionariosBimestres_DeveSerInicializadoComoListaVazia()
        {
            var bimestre = new Bimestre(1, "Primeiro Bimestre");

            Assert.NotNull(bimestre.QuestionariosBimestres);
            Assert.IsAssignableFrom<ICollection<SME.Sondagem.Dominio.Entidades.Questionario.QuestionarioBimestre>>(bimestre.QuestionariosBimestres);
            Assert.Empty(bimestre.QuestionariosBimestres);
        }

        #endregion
    }
}