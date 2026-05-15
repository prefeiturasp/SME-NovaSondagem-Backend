using Microsoft.EntityFrameworkCore;
using Xunit;
using SME.Sondagem.Dados.Mapeamentos;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class RegistrarMapeamentosTeste
    {
        [Fact]
        public void Deve_Registrar_Mapeamentos_Sem_Excecao()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new DbContext(options);
            var modelBuilder = new ModelBuilder();

            // Act & Assert
            var exception = Record.Exception(() => RegistrarMapeamentos.Registrar(modelBuilder));
            Assert.Null(exception);
        }
    }
}
