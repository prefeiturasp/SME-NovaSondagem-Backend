using SME.Sondagem.Infra.EnvironmentVariables;
using Xunit;

namespace SME.Sondagem.Infra.Teste.EnvironmentVariables
{
    public class TelemetriaOptionsTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var options = new TelemetriaOptions
            {
                ApplicationInsights = true,
                Apm = true
            };

            Assert.True(options.ApplicationInsights);
            Assert.True(options.Apm);
        }

        [Fact]
        public void Propriedades_Devem_Ser_False_Por_Padrao()
        {
            var options = new TelemetriaOptions();

            Assert.False(options.ApplicationInsights);
            Assert.False(options.Apm);
        }

        [Fact]
        public void Deve_Retornar_Secao_Correta()
        {
            Assert.Equal("Telemetria", TelemetriaOptions.Secao);
        }
    }
}
