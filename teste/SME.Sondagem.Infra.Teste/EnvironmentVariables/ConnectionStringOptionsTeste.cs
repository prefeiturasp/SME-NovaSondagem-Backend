using SME.Sondagem.Infra.EnvironmentVariables;
using Xunit;

namespace SME.Sondagem.Infra.Teste.EnvironmentVariables
{
    public class ConnectionStringOptionsTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var options = new ConnectionStringOptions
            {
                ApiSondagemExterna = "http://externa",
                ApiSondagem = "http://api",
                ApiSondagemLeitura = "http://leitura"
            };

            Assert.Equal("http://externa", options.ApiSondagemExterna);
            Assert.Equal("http://api", options.ApiSondagem);
            Assert.Equal("http://leitura", options.ApiSondagemLeitura);
        }

        [Fact]
        public void Propriedades_Devem_Ser_Nulas_Por_Padrao()
        {
            var options = new ConnectionStringOptions();

            Assert.Null(options.ApiSondagemExterna);
            Assert.Null(options.ApiSondagem);
            Assert.Null(options.ApiSondagemLeitura);
        }
    }
}
