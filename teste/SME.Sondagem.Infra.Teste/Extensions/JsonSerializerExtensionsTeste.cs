using SME.Sondagem.Infra.Extensions;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Extensions
{
    public class JsonSerializerExtensionsTeste
    {
        private class ClasseTeste
        {
            public string Nome { get; set; } = string.Empty;
            public int Idade { get; set; }
            public string? CampoNulo { get; set; }
        }

        [Fact]
        public void ConverterObjectStringPraObjeto_deve_retornar_default_quando_string_vazia()
        {
            var json = string.Empty;

            var resultado = json.ConverterObjectStringPraObjeto<ClasseTeste>();

            Assert.Null(resultado);
        }

        [Fact]
        public void ConverterObjectStringPraObjeto_deve_retornar_default_quando_string_nula()
        {
            string? json = null;

            var resultado = json!.ConverterObjectStringPraObjeto<ClasseTeste>();

            Assert.Null(resultado);
        }

        [Fact]
        public void ConverterObjectStringPraObjeto_deve_converter_json_para_objeto()
        {
            var json = @"{ ""nome"": ""Mônica"", ""idade"": 30 }";

            var resultado = json.ConverterObjectStringPraObjeto<ClasseTeste>();

            Assert.NotNull(resultado);
            Assert.Equal("Mônica", resultado.Nome);
            Assert.Equal(30, resultado.Idade);
        }

        [Fact]
        public void ConverterObjectStringPraObjeto_deve_ignorar_case_dos_campos()
        {
            var json = @"{ ""NOME"": ""Teste"", ""IDADE"": 20 }";

            var resultado = json.ConverterObjectStringPraObjeto<ClasseTeste>();

            Assert.NotNull(resultado);
            Assert.Equal("Teste", resultado.Nome);
            Assert.Equal(20, resultado.Idade);
        }

        [Fact]
        public void ConverterObjectParaJson_deve_retornar_string_vazia_quando_objeto_nulo()
        {
            object? obj = null;

            var resultado = obj!.ConverterObjectParaJson();

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void ConverterObjectParaJson_deve_serializar_objeto()
        {
            var obj = new ClasseTeste
            {
                Nome = "Teste",
                Idade = 25
            };

            var json = obj.ConverterObjectParaJson();

            var resultado = json.ConverterObjectStringPraObjeto<ClasseTeste>();
            
            Assert.NotNull(resultado);
            Assert.Equal("Teste", resultado.Nome);
            Assert.Equal(25, resultado.Idade);
        }

        [Fact]
        public void ConverterObjectParaJson_deve_ignorar_propriedades_nulas()
        {
            var obj = new ClasseTeste
            {
                Nome = "Teste",
                Idade = 25,
                CampoNulo = null
            };

            var json = obj.ConverterObjectParaJson();

            Assert.DoesNotContain("CampoNulo", json);
        }
    }
}
