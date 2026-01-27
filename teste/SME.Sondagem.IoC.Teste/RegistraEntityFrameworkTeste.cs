using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Infrastructure.Interfaces;
using Xunit;

namespace SME.Sondagem.IoC.Teste;

public class RegistraEntityFrameworkTeste
{
    [Fact]
    public void Registrar_DeveAdicionarDbContextAoServiceCollection()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddScoped<IServicoUsuario, ServicoUsuarioMock>();

        RegistraEntityFramework.Registrar(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<SondagemDbContext>();

        Assert.NotNull(dbContext);
        
        serviceProvider.Dispose();
    }

    [Fact]
    public void Registrar_DeveAdicionarDbContextGenericoAoServiceCollection()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddScoped<IServicoUsuario, ServicoUsuarioMock>();

        RegistraEntityFramework.Registrar(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<DbContext>();

        Assert.NotNull(dbContext);
        Assert.IsType<SondagemDbContext>(dbContext);
        
        serviceProvider.Dispose();
    }

    [Fact]
    public void Registrar_DeveConfigurarPostgresComConnectionString()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddScoped<IServicoUsuario, ServicoUsuarioMock>();

        RegistraEntityFramework.Registrar(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<SondagemDbContext>();

        Assert.NotNull(dbContext);
        Assert.True(dbContext.Database.IsNpgsql());
        
        serviceProvider.Dispose();
    }

    [Fact]
    public void Registrar_DeveConfigurarConnectionString_QuandoProvidaNoConfiguration()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddScoped<IServicoUsuario, ServicoUsuarioMock>();

        RegistraEntityFramework.Registrar(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<SondagemDbContext>();

        Assert.NotNull(dbContext);
        var connectionString = dbContext.Database.GetConnectionString();
        Assert.NotNull(connectionString);
        Assert.Contains("sondagem_test", connectionString);
        
        serviceProvider.Dispose();
    }

    [Fact]
    public void Registrar_DeveRegistrarDbContextComoScoped()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddScoped<IServicoUsuario, ServicoUsuarioMock>();

        RegistraEntityFramework.Registrar(services, configuration);

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(SondagemDbContext));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void Registrar_DeveRegistrarDbContextGenericoComoScoped()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddScoped<IServicoUsuario, ServicoUsuarioMock>();

        RegistraEntityFramework.Registrar(services, configuration);

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContext));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void Registrar_DevePermitirResolverDbContextMultiplasVezes()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddScoped<IServicoUsuario, ServicoUsuarioMock>();

        RegistraEntityFramework.Registrar(services, configuration);

        var serviceProvider = services.BuildServiceProvider();

        using var scope1 = serviceProvider.CreateScope();
        var dbContext1 = scope1.ServiceProvider.GetService<SondagemDbContext>();

        using var scope2 = serviceProvider.CreateScope();
        var dbContext2 = scope2.ServiceProvider.GetService<SondagemDbContext>();

        Assert.NotNull(dbContext1);
        Assert.NotNull(dbContext2);
        Assert.NotSame(dbContext1, dbContext2);
        
        serviceProvider.Dispose();
    }

    private static IConfiguration CriarConfigurationMock()
    {
        var configurationData = new Dictionary<string, string>
        {
            { "ConnectionStrings:SondagemConnection", "Host=localhost;Database=sondagem_test;Username=postgres;Password=postgres" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData!)
            .Build();
    }

    private sealed class ServicoUsuarioMock : IServicoUsuario
    {
        public string ObterUsuarioLogado() => "usuario_teste";
        public string ObterRFUsuarioLogado() => "1234567";
    }
}
