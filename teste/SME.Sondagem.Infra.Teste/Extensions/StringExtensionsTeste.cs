using SME.Sondagem.Infra.Extensions;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Extensions
{
    public class StringExtensionsTeste
    {
        [Fact]
        public void ConverterParaInt_deve_converter_string_numerica()
        {
            var valor = "123";

            var resultado = valor.ConverterParaInt();

            Assert.Equal(123, resultado);
        }

        [Fact]
        public void ConverterParaInt_deve_retornar_zero_quando_string_vazia()
        {
            var valor = string.Empty;

            var resultado = valor.ConverterParaInt();

            Assert.Equal(0, resultado);
        }

        [Fact]
        public void ConverterParaInt_deve_retornar_zero_quando_string_nula()
        {
            string? valor = null;

            var resultado = valor!.ConverterParaInt();

            Assert.Equal(0, resultado);
        }

        [Fact]
        public void ConverterParaInt_deve_retornar_zero_quando_string_nao_numerica()
        {
            var valor = "abc";

            var resultado = valor.ConverterParaInt();

            Assert.Equal(0, resultado);
        }

        [Fact]
        public void ConverterParaInt_deve_retornar_zero_quando_numero_decimal()
        {
            var valor = "10.5";

            var resultado = valor.ConverterParaInt();

            Assert.Equal(0, resultado);
        }

        [Fact]
        public void ConverterParaInt_deve_converter_numero_negativo()
        {
            var valor = "-20";

            var resultado = valor.ConverterParaInt();

            Assert.Equal(-20, resultado);
        }
    }
}
