using SME.Sondagem.Infra.Fila;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Fila
{
    public class RotasRabbitTeste
    {
        [Fact]
        public void Deve_Retornar_Valor_Correto_RotaLogs()
        {
            Assert.Equal("ApplicationLog", RotasRabbit.RotaLogs);
        }

        [Fact]
        public void Deve_Retornar_Valor_Correto_Log()
        {
            Assert.Equal("ApplicationLog", RotasRabbit.Log);
        }

        [Fact]
        public void Deve_Retornar_Valor_Correto_IniciarSync()
        {
            Assert.Equal("Sondagem.iniciar.sync", RotasRabbit.IniciarSync);
        }
    }
}
