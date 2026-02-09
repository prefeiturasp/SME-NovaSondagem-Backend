using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.API.Configuracoes;

[ExcludeFromCodeCoverage]
public static class RegistraDocumentacaoSwagger
{
    public static void Registrar(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sondagem API",
                Version = "1.0"
            });

            // =========================
            // JWT - Bearer
            // =========================
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Authorization: Bearer token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            });

            // =========================
            // API KEY - Integração
            // =========================
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "Informe a API Key de integração",
                Name = "x-api-sondagem-key",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            });

            c.OperationFilter<SecurityByControllerTypeOperationFilter>();
        });
    }
}
