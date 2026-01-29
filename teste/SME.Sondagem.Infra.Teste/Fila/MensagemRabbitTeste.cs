using SME.Sondagem.Infra.Fila;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Fila
{
    public class MensagemRabbitTeste
    {
        private class MensagemFake
        {
            public string Nome { get; set; } = string.Empty;
            public int Valor { get; set; }
        }

        [Fact]
        public void Deve_Criar_MensagemRabbit_Com_Propriedades()
        {
            var mensagem = new { Nome = "Teste", Valor = 42 };
            var correlacao = Guid.NewGuid();

            var msgRabbit = new MensagemRabbit(mensagem, correlacao);

            Assert.Equal(mensagem, msgRabbit.Mensagem);
            Assert.Equal(correlacao, msgRabbit.CodigoCorrelacao);
        }

        [Fact]
        public void Deve_ObterObjetoMensagem_Retornar_Objeto_Corretamente()
        {
            var mensagemObj = new MensagemFake { Nome = "Teste", Valor = 99 };
            var mensagemString = System.Text.Json.JsonSerializer.Serialize(mensagemObj);
            var correlacao = Guid.NewGuid();

            var msgRabbit = new MensagemRabbit(mensagemString, correlacao);

            // Supondo que ConverterObjectStringPraObjeto<T> usa System.Text.Json para desserializar
            var obj = msgRabbit.ObterObjetoMensagem<MensagemFake>();

            Assert.NotNull(obj);
            Assert.Equal("Teste", obj.Nome);
            Assert.Equal(99, obj.Valor);
        }
    }
}
