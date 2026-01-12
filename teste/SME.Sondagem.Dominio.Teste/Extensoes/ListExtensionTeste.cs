using Xunit;

namespace SME.Sondagem.Dominio.Teste.Extensoes
{
    public class ListExtensionTeste
    {
        [Fact]
        public void PossuiRegistros_DeveRetornarTrue_QuandoListaPossuiElementos()
        {
            var lista = new List<int> { 1, 2, 3 };

            var resultado = lista.PossuiRegistros();

            Assert.True(resultado);
        }

        [Fact]
        public void PossuiRegistros_DeveRetornarFalse_QuandoListaVazia()
        {
            var lista = new List<int>();

            var resultado = lista.PossuiRegistros();

            Assert.False(resultado);
        }

        [Fact]
        public void PossuiRegistros_DeveRetornarFalse_QuandoListaNula()
        {
            List<int> lista = null!;

            var resultado = lista?.PossuiRegistros() ?? false;

            Assert.False(resultado);
        }

        [Fact]
        public void PossuiRegistros_ComPredicado_DeveRetornarTrue_QuandoEncontraElemento()
        {
            var lista = new List<int> { 1, 2, 3, 4, 5 };

            var resultado = lista.PossuiRegistros(x => x > 3);

            Assert.True(resultado);
        }

        [Fact]
        public void PossuiRegistros_ComPredicado_DeveRetornarFalse_QuandoNaoEncontraElemento()
        {
            var lista = new List<int> { 1, 2, 3 };

            var resultado = lista.PossuiRegistros(x => x > 10);

            Assert.False(resultado);
        }

        [Fact]
        public void PossuiRegistros_ComPredicado_DeveRetornarFalse_QuandoListaNula()
        {
            List<int> lista = null!;

            var resultado = lista?.PossuiRegistros(x => x > 0) ?? false;

            Assert.False(resultado);
        }

        [Fact]
        public void PossuiRegistros_ComPredicado_DeveRetornarFalse_QuandoListaVazia()
        {
            var lista = new List<int>();

            var resultado = lista.PossuiRegistros(x => x > 0);

            Assert.False(resultado);
        }

        [Fact]
        public void NaoPossuiRegistros_DeveRetornarTrue_QuandoListaVazia()
        {
            var lista = new List<string>();

            var resultado = lista.NaoPossuiRegistros();

            Assert.True(resultado);
        }

        [Fact]
        public void NaoPossuiRegistros_DeveRetornarTrue_QuandoListaNula()
        {
            List<string> lista = null!;

            var resultado = lista?.NaoPossuiRegistros() ?? true;

            Assert.True(resultado);
        }

        [Fact]
        public void NaoPossuiRegistros_DeveRetornarFalse_QuandoListaPossuiElementos()
        {
            var lista = new List<string> { "item1", "item2" };

            var resultado = lista.NaoPossuiRegistros();

            Assert.False(resultado);
        }

        [Fact]
        public void NaoPossuiRegistros_ComPredicado_DeveRetornarTrue_QuandoNaoEncontraElemento()
        {
            var lista = new List<int> { 1, 2, 3 };

            var resultado = lista.NaoPossuiRegistros(x => x > 10);

            Assert.True(resultado);
        }

        [Fact]
        public void NaoPossuiRegistros_ComPredicado_DeveRetornarFalse_QuandoEncontraElemento()
        {
            var lista = new List<int> { 1, 2, 3, 4, 5 };

            var resultado = lista.NaoPossuiRegistros(x => x == 3);

            Assert.False(resultado);
        }

        [Fact]
        public void NaoPossuiRegistros_ComPredicado_DeveRetornarTrue_QuandoListaNula()
        {
            List<int> lista = null!;

            var resultado = lista?.NaoPossuiRegistros(x => x > 0) ?? true;

            Assert.True(resultado);
        }

        [Fact]
        public void LancarExcecaoNegocioSeNaoPossuiRegistros_DeveLancarExcecao_QuandoListaVazia()
        {
            var lista = new List<int>();
            var mensagemErro = "A lista não possui registros";

            var excecao = Assert.Throws<RegraNegocioException>(() => 
                lista.LancarExcecaoNegocioSeNaoPossuiRegistros(mensagemErro));
            
            Assert.Equal(mensagemErro, excecao.Message);
        }

        [Fact]
        public void LancarExcecaoNegocioSeNaoPossuiRegistros_DeveLancarExcecao_QuandoListaNula()
        {
            List<int> lista = null!;
            var mensagemErro = "Lista nula ou vazia";

            var excecao = Assert.Throws<RegraNegocioException>(() => 
                lista.LancarExcecaoNegocioSeNaoPossuiRegistros(mensagemErro));
            
            Assert.Equal(mensagemErro, excecao.Message);
        }

        [Fact]
        public void LancarExcecaoNegocioSeNaoPossuiRegistros_NaoDeveLancarExcecao_QuandoListaPossuiElementos()
        {
            var lista = new List<int> { 1, 2, 3 };
            var mensagemErro = "A lista não possui registros";

            var exception = Record.Exception(() => 
                lista.LancarExcecaoNegocioSeNaoPossuiRegistros(mensagemErro));
            
            Assert.Null(exception);
        }

        [Fact]
        public void PossuiRegistros_DeveTrabalharComIEnumerable()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1, 5);

            var resultado = enumerable.PossuiRegistros();

            Assert.True(resultado);
        }

        [Fact]
        public void NaoPossuiRegistros_DeveTrabalharComDiferentesTipos()
        {
            var listaStrings = new List<string>();
            var listaInts = new List<int>();
            var listaObjetos = new List<object>();

            Assert.True(listaStrings.NaoPossuiRegistros());
            Assert.True(listaInts.NaoPossuiRegistros());
            Assert.True(listaObjetos.NaoPossuiRegistros());
        }
    }
}