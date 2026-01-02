using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SME.Sondagem.Infra.Constantes.Autenticacao;
using static System.Text.Encoding;

namespace SME.SME.Sondagem.Api.Configuracoes;

public static class RegistraAutenticacao
{
    public static void Registrar(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(AutenticacaoSettingsApi.BearerTokenSGP, o =>
        {
            var issuerSigningKey = configuration.GetValue<string>("SGPApiTokenSettings:IssuerSigningKey")
                ?? throw new InvalidOperationException("SGPApiTokenSettings:IssuerSigningKey não configurado");

            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidAudience = configuration.GetValue<string>("SGPApiTokenSettings:Audience"),
                ValidateIssuer = true,
                ValidIssuer = configuration.GetValue<string>("SGPApiTokenSettings:Issuer"),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(UTF8.GetBytes(issuerSigningKey))
            };
            o.MapInboundClaims = false;
        })
        .AddJwtBearer(AutenticacaoSettingsApi.BearerTokenSondagem, options =>
        {
            var issuerSigningKey = configuration.GetValue<string>("SondagemTokenSettings:IssuerSigningKey")
                ?? throw new InvalidOperationException("SondagemTokenSettings:IssuerSigningKey não configurado");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration.GetValue<string>("SondagemTokenSettings:Issuer"),
                ValidAudience = configuration.GetValue<string>("SondagemTokenSettings:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(UTF8.GetBytes(issuerSigningKey))
            };
        });

        services.AddAuthorization(auth =>
        {
            auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());
        });
    }
}