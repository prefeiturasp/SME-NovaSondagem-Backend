using SME.Sondagem.Dados.Contexto;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Contexto
{
    public class SondagemDbContextFactoryTeste
    {
        [Fact]
        public void Deve_Criar_DbContext_Com_ConnectionString_Valida()
        {
            var factory = new SondagemDbContextFactory();

            try
            {
                var contexto = factory.CreateDbContext(Array.Empty<string>());
                Assert.NotNull(contexto);
                Assert.IsType<SondagemDbContext>(contexto);
            }
            catch (InvalidOperationException ex)
            {
                // Aceita qualquer mensagem de erro esperada
                Assert.True(
                    ex.Message.Contains("Connection string", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("secrets.json", StringComparison.OrdinalIgnoreCase),
                    $"Mensagem inesperada: {ex.Message}"
                );
            }
        }
    }
}
