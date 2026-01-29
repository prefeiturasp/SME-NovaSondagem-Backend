using SME.Sondagem.Infra.Fila;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Fila
{
    public class ComandoRabbitTeste
    {
        [Fact]
        public void Deve_Criar_ComandoRabbit_Com_Parametros_Padrao()
        {
            var comando = new ComandoRabbit("ProcessoA", typeof(string));

            Assert.Equal("ProcessoA", comando.NomeProcesso);
            Assert.Equal(typeof(string), comando.TipoCasoUso);
            Assert.Equal((ulong)3, comando.QuantidadeReprocessamentoDeadLetter);
            Assert.Equal(10 * 60 * 100, comando.Ttl);
            Assert.False(comando.ModeLazy);
        }

        [Fact]
        public void Deve_Criar_ComandoRabbit_Com_ModeLazy()
        {
            var comando = new ComandoRabbit("ProcessoB", typeof(int), true);

            Assert.Equal("ProcessoB", comando.NomeProcesso);
            Assert.Equal(typeof(int), comando.TipoCasoUso);
            Assert.Equal((ulong)3, comando.QuantidadeReprocessamentoDeadLetter);
            Assert.Equal(10 * 60 * 100, comando.Ttl);
            Assert.True(comando.ModeLazy);
        }

        [Fact]
        public void Deve_Criar_ComandoRabbit_Com_Todos_Parametros()
        {
            var comando = new ComandoRabbit("ProcessoC", typeof(DateTime), true, 10, 5000);

            Assert.Equal("ProcessoC", comando.NomeProcesso);
            Assert.Equal(typeof(DateTime), comando.TipoCasoUso);
            Assert.Equal((ulong)10, comando.QuantidadeReprocessamentoDeadLetter);
            Assert.Equal(5000, comando.Ttl);
            Assert.True(comando.ModeLazy);
        }
    }
}
