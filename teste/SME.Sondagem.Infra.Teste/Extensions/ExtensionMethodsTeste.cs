using SME.Sondagem.Infra.Extensions;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Extensions
{
    public class ExtensionMethodsTeste
    {
        #region ObterConstantesPublicas

        private class ClasseConstantes
        {
            public const int ConstanteInt = 10;
            public const string ConstanteString = "ABC";

            public static readonly int ReadOnly = 99;
        }

        [Fact]
        public void ObterConstantesPublicas_deve_retornar_constantes_publicas_do_tipo()
        {
            var resultado = typeof(ClasseConstantes).ObterConstantesPublicas<int>();

            Assert.Single(resultado);
            Assert.Contains(10, resultado);
        }

        [Fact]
        public void ObterConstantesPublicas_deve_retornar_lista_vazia_quando_nao_existir_constantes_do_tipo()
        {
            var resultado = typeof(ClasseConstantes).ObterConstantesPublicas<DateTime>();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        #endregion

        #region ObterMetodo

        private interface ITesteInterface
        {
            void MetodoInterface();
        }

        private class ClasseComInterface : ITesteInterface
        {
            public void MetodoInterface() { }

            public static void MetodoClasse() { }
        }

        [Fact]
        public void ObterMetodo_deve_encontrar_metodo_na_propria_classe()
        {
            var metodo = typeof(ClasseComInterface).ObterMetodo("MetodoClasse");

            Assert.NotNull(metodo);
            Assert.Equal("MetodoClasse", metodo.Name);
        }

        [Fact]
        public void ObterMetodo_deve_encontrar_metodo_definido_na_interface()
        {
            var metodo = typeof(ClasseComInterface).ObterMetodo("MetodoInterface");

            Assert.NotNull(metodo);
            Assert.Equal("MetodoInterface", metodo.Name);
        }

        #endregion

        #region InvokeAsync

        private class ClasseAsync
        {
            public static async Task<string> MetodoAsync()
            {
                await Task.Delay(10);
                return "OK";
            }

            public static async Task MetodoAsyncSemRetorno()
            {
                await Task.Delay(10);
            }

            public static string MetodoSync()
            {
                return "SYNC";
            }
        }

        [Fact]
        public async Task InvokeAsync_deve_executar_metodo_async_e_retornar_resultado()
        {
            var instancia = new ClasseAsync();
            var metodo = typeof(ClasseAsync).GetMethod(nameof(ClasseAsync.MetodoAsync))!;

            var resultado = await metodo.InvokeAsync(instancia);

            Assert.Equal("OK", resultado);
        }

        [Fact]
        public async Task InvokeAsync_deve_lancar_excecao_quando_metodo_async_nao_possui_retorno()
        {
            // Arrange
            var instancia = new ClasseAsync();
            var metodo = typeof(ClasseAsync)
                .GetMethod(nameof(ClasseAsync.MetodoAsyncSemRetorno))!;

            var exception = await Assert.ThrowsAsync<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(
                async () => await metodo.InvokeAsync(instancia)
            );

            Assert.Contains("Cannot implicitly convert type 'void' to 'object'", exception.Message);
        }

        [Fact]
        public async Task InvokeAsync_deve_lancar_excecao_quando_metodo_sync()
        {
            var instancia = new ClasseAsync();
            var metodo = typeof(ClasseAsync)
                .GetMethod(nameof(ClasseAsync.MetodoSync))!;

            var exception = await Assert.ThrowsAsync<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(
                async () => await metodo.InvokeAsync(instancia)
            );

            Assert.Contains("GetAwaiter", exception.Message);
        }
        #endregion
    }
}
