using SME.Sondagem.Infra.EnvironmentVariables;
using StackExchange.Redis;
using Xunit;

namespace SME.Sondagem.Infra.Teste.EnvironmentVariables
{
    public class RedisOptionsTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var options = new RedisOptions
            {
                Endpoint = "localhost:6379",
                SyncTimeout = 10000,
                Proxy = Proxy.Twemproxy
            };

            Assert.Equal("localhost:6379", options.Endpoint);
            Assert.Equal(10000, options.SyncTimeout);
            Assert.Equal(Proxy.Twemproxy, options.Proxy);
        }

        [Fact]
        public void Propriedades_Devem_Ter_Valores_Padrao()
        {
            var options = new RedisOptions();

            Assert.Equal(string.Empty, options.Endpoint);
            Assert.Equal(5000, options.SyncTimeout);
            Assert.Equal(Proxy.None, options.Proxy);
        }

        [Fact]
        public void Deve_Retornar_Secao_Correta()
        {
            Assert.Equal("Redis", RedisOptions.Secao);
        }
    }
}
