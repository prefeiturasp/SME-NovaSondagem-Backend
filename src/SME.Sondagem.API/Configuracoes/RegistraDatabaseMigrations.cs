using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;

namespace SME.Sondagem.API.Configuracoes
{
    public static class RegistraDatabaseMigrations
    {
        public static async Task Registrar(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SondagemDbContext>();

                try
                {
                    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

                    if (pendingMigrations.Any())
                    {
                        await dbContext.Database.MigrateAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao aplicar migrations: {ex.Message}");
                }
            }
        }
    }
}
