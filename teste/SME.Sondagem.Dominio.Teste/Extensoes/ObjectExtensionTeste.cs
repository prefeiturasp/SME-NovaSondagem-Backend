using Xunit;

namespace SME.Sondagem.Dominio.Teste.Extensoes
{
    public class ObjectExtensionTeste
    {
        [Fact]
        public void LancarExcecaoNegocioSeEhNulo_Deve_lancar_excecao_quando_objeto_eh_nulo()
        {
            object? objetoNulo = null;
            var mensagemErro = "Objeto não pode ser nulo";

            var excecao = Assert.Throws<RegraNegocioException>(() => 
                objetoNulo!.LancarExcecaoNegocioSeEhNulo(mensagemErro));

            Assert.Equal(mensagemErro, excecao.Message);
        }

        [Fact]
        public void LancarExcecaoNegocioSeEhNulo_Nao_deve_lancar_excecao_quando_objeto_nao_eh_nulo()
        {
            var objeto = new object();
            var mensagemErro = "Objeto não pode ser nulo";

            var exception = Record.Exception(() => 
                objeto.LancarExcecaoNegocioSeEhNulo(mensagemErro));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Olá", "Ola")]
        [InlineData("São Paulo", "Sao Paulo")]
        [InlineData("José", "Jose")]
        [InlineData("Açucar", "Acucar")]
        [InlineData("Ação", "Acao")]
        [InlineData("Coração", "Coracao")]
        [InlineData("Pêssego", "Pessego")]
        [InlineData("Água", "Agua")]
        [InlineData("Avião", "Aviao")]
        public void RemoverAcentuacao_Deve_remover_acentos_corretamente(string entrada, string esperado)
        {
            var resultado = entrada.RemoverAcentuacao();

            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void RemoverAcentuacao_Deve_retornar_string_vazia_quando_entrada_vazia()
        {
            var entrada = string.Empty;

            var resultado = entrada.RemoverAcentuacao();

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void RemoverAcentuacao_Deve_retornar_null_quando_entrada_null()
        {
            string? entrada = null;

            var resultado = entrada?.RemoverAcentuacao()!;

            Assert.Null(resultado);
        }

        [Fact]
        public void RemoverAcentuacao_Deve_manter_caracteres_sem_acento()
        {
            var entrada = "Teste sem acento";

            var resultado = entrada.RemoverAcentuacao();

            Assert.Equal(entrada, resultado);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData(" ", false)]
        [InlineData("teste", false)]
        public void ItemSemPreenchimento_Deve_identificar_string_vazia_ou_nula(string? entrada, bool esperado)
        {
            var resultado = entrada.ItemSemPreenchimento();

            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData("Teste", "teste", false)]
        [InlineData("TESTE", "teste", false)]
        [InlineData("teste", "TESTE", false)]
        [InlineData("Teste", "Teste", false)]
        [InlineData("Teste", "Outro", true)]
        [InlineData("ABC", "XYZ", true)]
        [InlineData("São Paulo", "são paulo", false)]
        public void SaoDiferentes_Deve_comparar_strings_ignorando_case(string valor, string valorAComparar, bool esperado)
        {
            var resultado = valor.SaoDiferentes(valorAComparar);

            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData("", "", false)]
        [InlineData(" ", "  ", true)]
        [InlineData("a", "b", true)]
        public void SaoDiferentes_Deve_tratar_casos_especiais(string valor, string valorAComparar, bool esperado)
        {
            var resultado = valor.SaoDiferentes(valorAComparar);

            Assert.Equal(esperado, resultado);
        }
    }
}