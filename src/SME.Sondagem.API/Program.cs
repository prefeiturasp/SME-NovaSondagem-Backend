using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using SME.SME.Sondagem.Api.Configuracoes;
using SME.Sondagem.API.Configuracoes;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.IoC;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var conexaoDadosVariaveis = new ConnectionStringOptions();
builder.Configuration.GetSection("ConnectionStrings").Bind(conexaoDadosVariaveis, c => c.BindNonPublicProperties = true);
builder.Services.AddSingleton(conexaoDadosVariaveis);

RegistraEntityFramework.Registrar(builder.Services, builder.Configuration);

var telemetriaOptions = new TelemetriaOptions();
builder.Configuration.GetSection(TelemetriaOptions.Secao).Bind(telemetriaOptions, c => c.BindNonPublicProperties = true);
builder.Services.AddSingleton(telemetriaOptions);

var servicoTelemetria = new ServicoTelemetria(telemetriaOptions);
builder.Services.AddSingleton(servicoTelemetria);

var rabbitOptions = new RabbitOptions();
builder.Configuration.GetSection("Rabbit").Bind(rabbitOptions, c => c.BindNonPublicProperties = true);
builder.Services.AddSingleton(rabbitOptions);

builder.Services.AddSingleton(_ =>
{
    var factory = new ConnectionFactory
    {
        HostName = rabbitOptions.HostName,
        UserName = rabbitOptions.UserName,
        Password = rabbitOptions.Password,
        VirtualHost = rabbitOptions.VirtualHost
    };
    return factory.CreateConnectionAsync().Result;
});

var configuracaoRabbitLogOptions = new RabbitLogOptions();
builder.Configuration.GetSection("RabbitLog").Bind(configuracaoRabbitLogOptions, c => c.BindNonPublicProperties = true);
builder.Services.AddSingleton(configuracaoRabbitLogOptions);

var redisOptions = new RedisOptions();
builder.Configuration.GetSection(RedisOptions.Secao).Bind(redisOptions, c => c.BindNonPublicProperties = true);

var redisConfigurationOptions = new ConfigurationOptions()
{
    Proxy = redisOptions.Proxy,
    SyncTimeout = redisOptions.SyncTimeout,
    EndPoints = { redisOptions.Endpoint }
};

var muxer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
builder.Services.AddSingleton<IConnectionMultiplexer>(muxer);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

RegistraAutenticacao.Registrar(builder.Services, builder.Configuration);
RegistraDocumentacaoSwagger.Registrar(builder.Services);
RegistraDependencias.Registrar(builder.Services);
RegistraRepositorios.Registrar(builder.Services);

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    else
    {
        var allowedOriginsString = builder.Configuration["Cors:AllowedOrigins"];
        var allowedOrigins = string.IsNullOrWhiteSpace(allowedOriginsString)
            ? Array.Empty<string>()
            : allowedOriginsString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        options.AddPolicy("CorsPolicy", policy =>
        {
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
            else
            {
                policy.WithOrigins("https://localhost")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
        });
    }
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SondagemDbContext>();

    try
    {
        Console.WriteLine("?? Verificando migrations pendentes...");
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            Console.WriteLine($"?? Aplicando {pendingMigrations.Count()} migration(s)...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("? Migrations aplicadas com sucesso!");
        }
        else
        {
            Console.WriteLine("? Banco de dados est� atualizado!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Erro ao aplicar migrations: {ex.Message}");
        Console.WriteLine($"   Stack: {ex.StackTrace}");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SME Sondagem API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("?? SME Sondagem API iniciada com sucesso!");
logger.LogInformation($"?? Ambiente: {app.Environment.EnvironmentName}");
logger.LogInformation($"?? Connection String configurada: {!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("SondagemConnection"))}");

app.Run();