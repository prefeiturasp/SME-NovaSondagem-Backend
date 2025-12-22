using Microsoft.ApplicationInsights;
using RabbitMQ.Client;
using SME.SME.Sondagem.Api.Configurations;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.IoC;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var conexaoDadosVariaveis = new ConnectionStringOptions();
builder.Configuration.GetSection("ConnectionStrings").Bind(conexaoDadosVariaveis, c => c.BindNonPublicProperties = true);
builder.Services.AddSingleton(conexaoDadosVariaveis);

var telemetriaOptions = new TelemetriaOptions();
builder.Configuration.GetSection(TelemetriaOptions.Secao).Bind(telemetriaOptions, c => c.BindNonPublicProperties = true);
builder.Services.AddSingleton(telemetriaOptions);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serviceProvider = builder.Services.BuildServiceProvider();
var clientTelemetry = serviceProvider.GetService<TelemetryClient>();
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

//comentado o uso do Redis temporariamente
//var muxer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
//builder.Services.AddSingleton<IConnectionMultiplexer>(muxer);

builder.Services.AddHttpContextAccessor();
//builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);

RegistraAutenticacao.Registrar(builder.Services, builder.Configuration);
RegistraDocumentacaoSwagger.Registrar(builder.Services);
RegistraDependencias.Registrar(builder.Services);
RegistraRepositorios.Registrar(builder.Services);
RegistraUseCases.Registrar(builder.Services);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();

app.MapControllers();

app.Run();