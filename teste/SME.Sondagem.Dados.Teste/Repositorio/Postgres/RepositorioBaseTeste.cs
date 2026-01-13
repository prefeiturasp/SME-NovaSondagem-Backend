using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;

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
    }
}
