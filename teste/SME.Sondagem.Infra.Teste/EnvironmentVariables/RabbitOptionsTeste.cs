using SME.Sondagem.Infra.EnvironmentVariables;
using Xunit;

namespace SME.Sondagem.Infra.Teste.EnvironmentVariables
{
    public class RabbitOptionsTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var options = new RabbitOptions
            {
                HostName = "localhost",
                UserName = "user",
                Password = "senha",
                VirtualHost = "vh",
                LimiteDeMensagensPorExecucao = 10
            };

            Assert.Equal("localhost", options.HostName);
            Assert.Equal("user", options.UserName);
            Assert.Equal("senha", options.Password);
            Assert.Equal("vh", options.VirtualHost);
            Assert.Equal((ushort)10, options.LimiteDeMensagensPorExecucao);
        }

        [Fact]
        public void Propriedades_Devem_Ser_Nulas_Ou_Zero_Por_Padrao()
        {
            var options = new RabbitOptions();

            Assert.Null(options.HostName);
            Assert.Null(options.UserName);
            Assert.Null(options.Password);
            Assert.Null(options.VirtualHost);
            Assert.Equal((ushort)0, options.LimiteDeMensagensPorExecucao);
        }
    }
}
