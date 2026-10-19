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
        [Theory]
        [InlineData(null)]
        [InlineData(4)]
        public async System.Threading.Tasks.Task Deve_Renovar_Auditoria_Ao_Salvar_Resposta_Igual(int? opcao)
        {
            var usuario = new Mock<IServicoUsuario>();
            usuario.Setup(x => x.ObterUsuarioLogado()).Returns("Primeiro usuário");
            usuario.Setup(x => x.ObterRFUsuarioLogado()).Returns("111");
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .AddInterceptors(new AuditoriaSaveChangesInterceptor(usuario.Object))
                .Options;
            using var contexto = new SondagemDbContext(options);
            var dados = new SME.Sondagem.Dominio.ValueObjects.ContextoEducacional { TurmaId = "1", BimestreId = 1 };
            var resposta = new SME.Sondagem.Dominio.Entidades.Sondagem.RespostaAluno(
                1, 2, 3, opcao, System.DateTime.UtcNow.AddDays(-1), dados);
            contexto.Add(resposta);
            await contexto.SaveChangesAsync();
            var criadoEm = resposta.CriadoEm;
            Assert.Null(resposta.AlteradoEm);

            usuario.Setup(x => x.ObterUsuarioLogado()).Returns("Segundo usuário");
            usuario.Setup(x => x.ObterRFUsuarioLogado()).Returns("222");
            var inicio = System.DateTime.UtcNow;
            resposta.AtualizarResposta(opcao, inicio, dados);
            await contexto.SaveChangesAsync();

            Assert.Equal(opcao, resposta.OpcaoRespostaId);
            Assert.Equal(criadoEm, resposta.CriadoEm);
            Assert.Equal("111", resposta.CriadoRF);
            Assert.Equal("222", resposta.AlteradoRF);
            Assert.Equal("Segundo usuário", resposta.AlteradoPor);
            Assert.True(resposta.AlteradoEm >= inicio);
            Assert.Equal(2, await contexto.Set<SME.Sondagem.Dominio.Entidades.Auditoria>().CountAsync());
        }

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
