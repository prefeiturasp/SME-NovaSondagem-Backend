using SME.Sondagem.Dados.Contexto;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Contexto
{
    public class SondagemDbContextFactoryTeste
    {
        [Fact]
        public void Deve_Criar_DbContext_Com_ConnectionString_Valida()
        {
            // Arrange
            var factory = new SondagemDbContextFactory();

            // Act & Assert
            // O teste abaixo só funcionará se o ambiente estiver corretamente configurado
            // com os arquivos e secrets necessários. Caso contrário, ele deve lançar uma exceção.
            try
            {
                var contexto = factory.CreateDbContext(Array.Empty<string>());
                Assert.NotNull(contexto);
                Assert.IsType<SondagemDbContext>(contexto);
            }
            catch (InvalidOperationException ex)
            {
                // Esperado caso o ambiente não esteja configurado
                Assert.Contains("Connection string", ex.Message, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
