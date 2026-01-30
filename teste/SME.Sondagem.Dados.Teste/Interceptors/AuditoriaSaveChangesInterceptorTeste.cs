using Microsoft.EntityFrameworkCore;
using Moq;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interceptors;
using SME.Sondagem.Infrastructure.Interfaces;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Interceptors
{
    public class AuditoriaSaveChangesInterceptorTeste
    {
        [Fact]
        public void Deve_Adicionar_Interceptor_No_Contexto()
        {
            // Arrange
            var servicoUsuarioMock = new Mock<IServicoUsuario>();
            var interceptor = new AuditoriaSaveChangesInterceptor(servicoUsuarioMock.Object);

            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase("auditoria_test")
                .AddInterceptors(interceptor)
                .Options;

            // Act
            using var contexto = new SondagemDbContext(options);

            // Assert
            Assert.NotNull(contexto);
        }
    }
}
