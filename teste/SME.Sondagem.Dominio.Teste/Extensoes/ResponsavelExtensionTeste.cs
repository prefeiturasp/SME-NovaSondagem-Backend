using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Extensoes
{
    public class ResponsavelExtensionTeste
    {
        [Theory]
        [InlineData("1", "Filiação 1")]
        public void ObterTipoResponsavel_Deve_retornar_filiacao1_quando_tipo_for_1(string tipoResponsavel, string nomeEsperado)
        {
            var resultado = ResponsavelExtension.ObterTipoResponsavel(tipoResponsavel);

            Assert.Equal(nomeEsperado, resultado);
        }

        [Theory]
        [InlineData("2", "Filiação 2")]
        public void ObterTipoResponsavel_Deve_retornar_filiacao2_quando_tipo_for_2(string tipoResponsavel, string nomeEsperado)
        {
            var resultado = ResponsavelExtension.ObterTipoResponsavel(tipoResponsavel);

            Assert.Equal(nomeEsperado, resultado);
        }

        [Theory]
        [InlineData("3", "Responsável Legal")]
        public void ObterTipoResponsavel_Deve_retornar_responsavel_legal_quando_tipo_for_3(string tipoResponsavel, string nomeEsperado)
        {
            var resultado = ResponsavelExtension.ObterTipoResponsavel(tipoResponsavel);

            Assert.Equal(nomeEsperado, resultado);
        }

        [Theory]
        [InlineData("4", "Próprio estudante")]
        public void ObterTipoResponsavel_Deve_retornar_proprio_estudante_quando_tipo_for_4(string tipoResponsavel, string nomeEsperado)
        {
            var resultado = ResponsavelExtension.ObterTipoResponsavel(tipoResponsavel);

            Assert.Equal(nomeEsperado, resultado);
        }

        [Theory]
        [InlineData("5", "Responsável Estrangeiro ou Naturalizado")]
        public void ObterTipoResponsavel_Deve_retornar_responsavel_estrangeiro_quando_tipo_for_5(string tipoResponsavel, string nomeEsperado)
        {
            var resultado = ResponsavelExtension.ObterTipoResponsavel(tipoResponsavel);

            Assert.Equal(nomeEsperado, resultado);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("6")]
        [InlineData("99")]
        [InlineData("abc")]
        [InlineData("")]
        [InlineData(null)]
        public void ObterTipoResponsavel_Deve_retornar_filiacao1_toString_quando_tipo_for_invalido(string? tipoResponsavel)
        {
            var resultado = ResponsavelExtension.ObterTipoResponsavel(tipoResponsavel!);

            Assert.Equal(TipoResponsavel.Filiacao1.ToString(), resultado);
        }

        [Fact]
        public void ObterTipoResponsavel_Deve_cobrir_todos_os_tipos_validos()
        {
            var tipos = new Dictionary<string, string>
            {
                { "1", "Filiação 1" },
                { "2", "Filiação 2" },
                { "3", "Responsável Legal" },
                { "4", "Próprio estudante" },
                { "5", "Responsável Estrangeiro ou Naturalizado" }
            };

            foreach (var tipo in tipos)
            {
                var resultado = ResponsavelExtension.ObterTipoResponsavel(tipo.Key);
                Assert.Equal(tipo.Value, resultado);
            }
        }
    }
}