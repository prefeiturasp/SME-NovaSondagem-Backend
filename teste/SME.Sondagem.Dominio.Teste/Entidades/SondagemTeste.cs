using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class SondagemTeste
    {
        [Fact]
        public void Deve_criar_sondagem_com_descricao()
        {
            var descricao = "Sondagem de Matemática";
            var dataAplicacao = new DateTime(2025, 3, 15);

            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(descricao, dataAplicacao);

            Assert.Equal(descricao, sondagem.Descricao);
        }

        [Fact]
        public void Deve_criar_sondagem_com_data_aplicacao()
        {
            var descricao = "Sondagem de Português";
            var dataAplicacao = new DateTime(2025, 6, 20);

            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(descricao, dataAplicacao);

            Assert.Equal(dataAplicacao, sondagem.DataAplicacao);
        }

        [Fact]
        public void Deve_criar_sondagem_com_todos_os_dados()
        {
            var descricao = "Sondagem Completa";
            var dataAplicacao = new DateTime(2025, 12, 31);

            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(descricao, dataAplicacao);

            Assert.Equal(descricao, sondagem.Descricao);
            Assert.Equal(dataAplicacao, sondagem.DataAplicacao);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_respostas_vazia()
        {
            var sondagem = CriarSondagemPadrao();

            Assert.NotNull(sondagem.Respostas);
            Assert.Empty(sondagem.Respostas);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_periodos_bimestre_vazia()
        {
            var sondagem = CriarSondagemPadrao();

            Assert.NotNull(sondagem.PeriodosBimestre);
            Assert.Empty(sondagem.PeriodosBimestre);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_questionarios()
        {
            var sondagem = CriarSondagemPadrao();

            Assert.Null(sondagem.Questionarios);
        }

        [Fact]
        public void Deve_possuir_descricao_com_setter_privado()
        {
            var sondagemType = typeof(Dominio.Entidades.Sondagem.Sondagem);
            var descricaoProperty = sondagemType.GetProperty("Descricao");

            Assert.NotNull(descricaoProperty);
            Assert.NotNull(descricaoProperty.SetMethod);
            Assert.True(descricaoProperty.SetMethod.IsPrivate);
        }

        [Fact]
        public void Deve_possuir_data_aplicacao_com_setter_privado()
        {
            var sondagemType = typeof(Dominio.Entidades.Sondagem.Sondagem);
            var dataAplicacaoProperty = sondagemType.GetProperty("DataAplicacao");

            Assert.NotNull(dataAplicacaoProperty);
            Assert.NotNull(dataAplicacaoProperty.SetMethod);
            Assert.True(dataAplicacaoProperty.SetMethod.IsPrivate);
        }

        [Fact]
        public void Deve_possuir_questionarios_com_setter_privado()
        {
            var sondagemType = typeof(Dominio.Entidades.Sondagem.Sondagem);
            var questionariosProperty = sondagemType.GetProperty("Questionarios");

            Assert.NotNull(questionariosProperty);
            Assert.NotNull(questionariosProperty.SetMethod);
            Assert.True(questionariosProperty.SetMethod.IsPrivate);
        }

        [Fact]
        public void Deve_possuir_respostas_com_setter_privado()
        {
            var sondagemType = typeof(Dominio.Entidades.Sondagem.Sondagem);
            var respostasProperty = sondagemType.GetProperty("Respostas");

            Assert.NotNull(respostasProperty);
            Assert.NotNull(respostasProperty.SetMethod);
            Assert.True(respostasProperty.SetMethod.IsPrivate);
        }

        [Fact]
        public void Deve_possuir_periodos_bimestre_com_setter_privado()
        {
            var sondagemType = typeof(Dominio.Entidades.Sondagem.Sondagem);
            var periodosBimestreProperty = sondagemType.GetProperty("PeriodosBimestre");

            Assert.NotNull(periodosBimestreProperty);
            Assert.NotNull(periodosBimestreProperty.SetMethod);
            Assert.True(periodosBimestreProperty.SetMethod.IsPrivate);
        }

        [Fact]
        public void Deve_herdar_de_entidade_base()
        {
            var sondagem = CriarSondagemPadrao();

            Assert.IsType<EntidadeBase>(sondagem, exactMatch: false);
        }

        [Fact]
        public void Deve_ter_colecao_de_respostas_do_tipo_icollection()
        {
            var sondagemType = typeof(Dominio.Entidades.Sondagem.Sondagem);
            var respostasProperty = sondagemType.GetProperty("Respostas");

            Assert.NotNull(respostasProperty);
            Assert.True(typeof(ICollection<RespostaAluno>).IsAssignableFrom(respostasProperty.PropertyType));
        }

        [Fact]
        public void Deve_ter_colecao_de_periodos_bimestre_do_tipo_icollection()
        {
            var sondagemType = typeof(Dominio.Entidades.Sondagem.Sondagem);
            var periodosBimestreProperty = sondagemType.GetProperty("PeriodosBimestre");

            Assert.NotNull(periodosBimestreProperty);
            Assert.True(typeof(ICollection<SondagemPeriodoBimestre>).IsAssignableFrom(periodosBimestreProperty.PropertyType));
        }

        [Fact]
        public void Deve_ter_colecao_de_questionarios_do_tipo_icollection()
        {
            var sondagemType = typeof(Dominio.Entidades.Sondagem.Sondagem);
            var questionariosProperty = sondagemType.GetProperty("Questionarios");

            Assert.NotNull(questionariosProperty);
            Assert.True(typeof(ICollection<Questionario>).IsAssignableFrom(questionariosProperty.PropertyType));
        }

        [Fact]
        public void Deve_aceitar_descricao_vazia()
        {
            var descricao = string.Empty;
            var dataAplicacao = DateTime.Now;

            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(descricao, dataAplicacao);

            Assert.Empty(sondagem.Descricao);
        }

        [Fact]
        public void Deve_aceitar_diferentes_datas_de_aplicacao()
        {
            var descricao = "Teste";
            var dataAplicacao = new DateTime(2020, 1, 1);

            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(descricao, dataAplicacao);

            Assert.Equal(dataAplicacao, sondagem.DataAplicacao);
        }

        [Fact]
        public void Deve_criar_instancias_diferentes()
        {
            var sondagem1 = CriarSondagemPadrao();
            var sondagem2 = CriarSondagemPadrao();

            Assert.NotSame(sondagem1, sondagem2);
        }

        private static Dominio.Entidades.Sondagem.Sondagem CriarSondagemPadrao()
        {
            return new Dominio.Entidades.Sondagem.Sondagem("Teste", DateTime.Now);
        }
    }
}
