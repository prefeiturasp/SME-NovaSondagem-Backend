using Microsoft.EntityFrameworkCore;
using SME.SME.Sondagem.Api.Configuracoes;
using SME.Sondagem.API.Configuracoes;
using SME.Sondagem.API.Middlewares;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.IoC;
using System.Diagnostics.CodeAnalysis;


[assembly: ExcludeFromCodeCoverage]
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

var configuracaoRabbitLogOptions = new RabbitLogOptions();
builder.Configuration.GetSection("RabbitLog").Bind(configuracaoRabbitLogOptions, c => c.BindNonPublicProperties = true);
builder.Services.AddSingleton(configuracaoRabbitLogOptions);

var redisOptions = new RedisOptions();
builder.Configuration.GetSection(RedisOptions.Secao).Bind(redisOptions, c => c.BindNonPublicProperties = true);
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

RegistraCache.Registrar(builder.Services, redisOptions);
RegistraMensageria.Registrar(builder.Services, rabbitOptions);
RegistraAutenticacao.Registrar(builder.Services, builder.Configuration);
RegistraDocumentacaoSwagger.Registrar(builder.Services);
RegistraDependencias.Registrar(builder.Services, builder.Configuration);
RegistraRepositorios.Registrar(builder.Services);
RegistraConfiguracaoCors.Registrar(builder);
RegistraApiEol.Registrar(builder.Services, builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

await RegistraDatabaseMigrations.Registrar(app);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SME Sondagem API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
