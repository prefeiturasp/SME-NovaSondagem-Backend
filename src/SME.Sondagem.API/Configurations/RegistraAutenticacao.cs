using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SME.Sondagem.API.Constantes.Autenticacao;
using System.Text;
using static System.Text.Encoding;

namespace SME.SME.Sondagem.Api.Configurations
{
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
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateLifetime = true,
                       ValidateAudience = true,
                       ValidAudience = configuration.GetValue<string>("SGPApiTokenSettings:Audience"),
                       ValidateIssuer = true,
                       ValidIssuer = configuration.GetValue<string>("SGPApiTokenSettings:Issuer"),
                       ValidateIssuerSigningKey = true,
                       ClockSkew = TimeSpan.Zero,
                       IssuerSigningKey = new SymmetricSecurityKey(UTF8
                           .GetBytes(configuration.GetValue<string>("SGPApiTokenSettings:IssuerSigningKey")))
                   };
                   o.MapInboundClaims = false;
               })
           .AddJwtBearer(AutenticacaoSettingsApi.BearerTokenSondagem, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetValue<string>("SondagemTokenSettings:Issuer"),
                    ValidAudience = configuration.GetValue<string>("SondagemTokenSettings:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(UTF8.GetBytes(configuration.GetValue<string>("SondagemTokenSettings:IssuerSigningKey")))
                };
            });
;

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()

                   .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                   .RequireAuthenticatedUser()
                   .Build());
            });
        }
    }
}
