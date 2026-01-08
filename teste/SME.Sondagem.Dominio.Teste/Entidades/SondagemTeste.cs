using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class SondagemTeste
    {
        [Fact]
        public void Deve_criar_sondagem_com_dados_validos()
        {
            var questionarioId = 10;
            var dataAplicacao = new DateTime(2026, 1, 8);

            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(questionarioId, dataAplicacao);

            Assert.Equal(questionarioId, sondagem.QuestionarioId);
            Assert.Equal(dataAplicacao, sondagem.DataAplicacao);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_respostas()
        {
            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(5, DateTime.Now);

            Assert.NotNull(sondagem.Respostas);
            Assert.Empty(sondagem.Respostas);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_periodos_bimestre()
        {
            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(3, DateTime.Now);

            Assert.NotNull(sondagem.PeriodosBimestre);
            Assert.Empty(sondagem.PeriodosBimestre);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_questionario()
        {
            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(7, DateTime.Now);

            Assert.NotNull(sondagem);
            Assert.Null(sondagem.Questionario);
        }

        [Fact]
        public void Deve_herdar_propriedades_da_entidade_base()
        {
            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(15, DateTime.Now);

            Assert.Equal(0, sondagem.Id);
            Assert.Null(sondagem.AlteradoEm);
            Assert.Null(sondagem.AlteradoPor);
            Assert.Null(sondagem.AlteradoRF);
            Assert.NotEqual(default(DateTime), sondagem.CriadoEm);
            Assert.Equal(string.Empty, sondagem.CriadoPor);
            Assert.Equal(string.Empty, sondagem.CriadoRF);
            Assert.False(sondagem.Excluido);
        }
    }
}
