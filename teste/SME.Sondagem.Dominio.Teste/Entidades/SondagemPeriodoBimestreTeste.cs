using SME.Sondagem.Dominio.Entidades.Sondagem;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class SondagemPeriodoBimestreTeste
    {
        [Fact]
        public void Deve_criar_sondagem_periodo_bimestre_com_dados_validos()
        {
            var sondagemId = 10;
            var bimestreId = 5;
            var dataInicio = new DateTime(2026, 1, 1);
            var dataFim = new DateTime(2026, 6, 30);

            var sondagemPeriodoBimestre = new SondagemPeriodoBimestre(sondagemId, bimestreId, dataInicio, dataFim);

            Assert.Equal(sondagemId, sondagemPeriodoBimestre.SondagemId);
            Assert.Equal(bimestreId, sondagemPeriodoBimestre.BimestreId);
            Assert.Equal(dataInicio, sondagemPeriodoBimestre.DataInicio);
            Assert.Equal(dataFim, sondagemPeriodoBimestre.DataFim);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_sondagem()
        {
            var sondagemPeriodoBimestre = new SondagemPeriodoBimestre(7, 3, DateTime.Now, DateTime.Now.AddMonths(3));

            Assert.NotNull(sondagemPeriodoBimestre);
            Assert.Null(sondagemPeriodoBimestre.Sondagem);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_bimestre()
        {
            var sondagemPeriodoBimestre = new SondagemPeriodoBimestre(8, 4, DateTime.Now, DateTime.Now.AddMonths(6));

            Assert.NotNull(sondagemPeriodoBimestre);
            Assert.Null(sondagemPeriodoBimestre.Bimestre);
        }

        [Fact]
        public void Deve_herdar_propriedades_da_entidade_base()
        {
            var sondagemPeriodoBimestre = new SondagemPeriodoBimestre(15, 2, DateTime.Now, DateTime.Now.AddDays(30));
            sondagemPeriodoBimestre.CriadoEm = DateTime.UtcNow;
            Assert.Equal(0, sondagemPeriodoBimestre.Id);
            Assert.Null(sondagemPeriodoBimestre.AlteradoEm);
            Assert.Null(sondagemPeriodoBimestre.AlteradoPor);
            Assert.Null(sondagemPeriodoBimestre.AlteradoRF);
            Assert.NotEqual(default(DateTime), sondagemPeriodoBimestre.CriadoEm);
            Assert.Equal(string.Empty, sondagemPeriodoBimestre.CriadoPor);
            Assert.Equal(string.Empty, sondagemPeriodoBimestre.CriadoRF);
            Assert.False(sondagemPeriodoBimestre.Excluido);
        }
    }
}
