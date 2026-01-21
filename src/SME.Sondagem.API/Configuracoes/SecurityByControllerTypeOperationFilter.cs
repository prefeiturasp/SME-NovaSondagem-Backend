using Microsoft.OpenApi.Models;
using SME.Sondagem.API.Middlewares;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.API.Configuracoes;

[ExcludeFromCodeCoverage]
public class SecurityByControllerTypeOperationFilter : IOperationFilter
{
    private static readonly string[] EmptyStringArray = Array.Empty<string>();
    private static readonly string[] BearerArray = new[] { "Bearer" };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var isIntegracao =
            context.MethodInfo.DeclaringType!
                .GetCustomAttributes(true)
                .OfType<ChaveIntegracaoApiAttribute>()
                .Any()
            ||
            context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<ChaveIntegracaoApiAttribute>()
                .Any();

        operation.Security.Clear();

        if (isIntegracao)
        {
            // API KEY
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    EmptyStringArray
                }
            });
        }
        else
        {
            // JWT
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    BearerArray
                }
            });
        }
    }
}
