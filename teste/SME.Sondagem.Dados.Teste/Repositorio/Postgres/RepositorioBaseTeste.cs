using Microsoft.EntityFrameworkCore;
using Moq;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Teste.Services.Auditoria;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioBaseTeste
    {
        protected static SondagemDbContext CriarContexto(string nomeBanco)
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(nomeBanco)
                .EnableSensitiveDataLogging()
                .Options;

            return new SondagemDbContext(options);
        }

        protected static IServicoAuditoria CriarServicoAuditoria()
        {
            var mock = new Mock<IServicoAuditoria>();
            return mock.Object;
        }
        protected static ContextoFake CriarConextoBase()
        {
            var contexto = new ContextoFake();
            contexto.AdicionarVariaveis(new Dictionary<string, object>
                {
                    { "NomeUsuario", "Usuario Teste" },
                    { "RF", "123456" },
                    { "Administrador", "true" }
                });

            return contexto;
        }
    }
}