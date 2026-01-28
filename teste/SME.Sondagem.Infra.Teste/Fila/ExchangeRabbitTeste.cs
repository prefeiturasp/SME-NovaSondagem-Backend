using SME.Sondagem.Infra.Fila;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Fila
{
    public class ExchangeRabbitTeste
    {
        [Fact]
        public void Deve_Retornar_Valor_Correto_Sondagem()
        {
            Assert.Equal("Sondagem.workers", ExchangeRabbit.Sondagem);
        }

        [Fact]
        public void Deve_Retornar_Valor_Correto_SondagemEstudante()
        {
            Assert.Equal("Sondagem.estudante.workers", ExchangeRabbit.SondagemEstudante);
        }

        [Fact]
        public void Deve_Retornar_Valor_Correto_SondagemDeadLetter()
        {
            Assert.Equal("Sondagem.workers.deadletter", ExchangeRabbit.SondagemDeadLetter);
        }

        [Fact]
        public void Deve_Retornar_Valor_Correto_Logs()
        {
            Assert.Equal("EnterpriseApplicationLog", ExchangeRabbit.Logs);
        }

        [Fact]
        public void Deve_Retornar_Valor_Correto_SondagemDeadLetterTtl()
        {
            Assert.Equal(10 * 60 * 1000, ExchangeRabbit.SondagemDeadLetterTtl);
        }

        [Fact]
        public void Deve_Retornar_Valor_Correto_SondagemDeadLetterTtl_3()
        {
            Assert.Equal(3 * 60 * 1000, ExchangeRabbit.SondagemDeadLetterTtl_3);
        }
    }
}
