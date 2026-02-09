using SME.Sondagem.Infra.EnvironmentVariables;
using Xunit;

namespace SME.Sondagem.Infra.Teste.EnvironmentVariables
{
    public class RabbitLogOptionsTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var options = new RabbitLogOptions
            {
                HostName = "localhost",
                UserName = "user",
                Password = "senha",
                VirtualHost = "vh"
            };

            Assert.Equal("localhost", options.HostName);
            Assert.Equal("user", options.UserName);
            Assert.Equal("senha", options.Password);
            Assert.Equal("vh", options.VirtualHost);
        }

        [Fact]
        public void Propriedades_Devem_Ser_Nulas_Por_Padrao()
        {
            var options = new RabbitLogOptions();

            Assert.Null(options.HostName);
            Assert.Null(options.UserName);
            Assert.Null(options.Password);
            Assert.Null(options.VirtualHost);
        }
    }
}
