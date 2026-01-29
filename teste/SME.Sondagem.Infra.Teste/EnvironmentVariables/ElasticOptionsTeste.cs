using SME.Sondagem.Infra.EnvironmentVariables;
using Xunit;

namespace SME.Sondagem.Infra.Teste.EnvironmentVariables
{
    public class ElasticOptionsTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var options = new ElasticOptions
            {
                Urls = "http://localhost:9200",
                DefaultIndex = "indice-padrao",
                PrefixIndex = "prefixo",
                Username = "usuario",
                Password = "senha"
            };

            Assert.Equal("http://localhost:9200", options.Urls);
            Assert.Equal("indice-padrao", options.DefaultIndex);
            Assert.Equal("prefixo", options.PrefixIndex);
            Assert.Equal("usuario", options.Username);
            Assert.Equal("senha", options.Password);
        }

        [Fact]
        public void Propriedades_Devem_Ser_Vazias_Por_Padrao()
        {
            var options = new ElasticOptions();

            Assert.Equal(string.Empty, options.Urls);
            Assert.Equal(string.Empty, options.DefaultIndex);
            Assert.Equal(string.Empty, options.PrefixIndex);
            Assert.Equal(string.Empty, options.Username);
            Assert.Equal(string.Empty, options.Password);
        }

        [Fact]
        public void Deve_Retornar_Secao_Correta()
        {
            Assert.Equal("ElasticSearch", ElasticOptions.Secao);
        }
    }
}
