using Microsoft.EntityFrameworkCore;
using Moq;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Services.Auditoria;

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
    }
}