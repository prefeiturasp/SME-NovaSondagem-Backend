using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.Sondagem.Dados.Contexto;

namespace SME.Sondagem.IoC
{
    public static class RegistraEntityFramework
    {
        public static void Registrar(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SondagemConnection");

            services.AddDbContext<SondagemDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly("SME.Sondagem.Dados");
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null);
                        npgsqlOptions.CommandTimeout(60);
                        npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history");
                    });

                // Configurações de desenvolvimento
#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif

                // Configurações de log
                options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            });

            // Registrar o DbContext como Scoped (padrão do EF Core)
            services.AddScoped<DbContext>(provider => provider.GetRequiredService<SondagemDbContext>());
        }
    }
}