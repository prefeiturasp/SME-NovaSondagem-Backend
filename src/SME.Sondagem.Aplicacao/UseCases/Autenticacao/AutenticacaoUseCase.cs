using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SME.Sondagem.Aplicacao.Interfaces.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.Autenticacao;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SME.Sondagem.Aplicacao.UseCases.Autenticacao;

public class AutenticacaoUseCase : IAutenticacaoUseCase
{
    private const string SGP_API_TOKEN_SCHEME = "SGPApiTokenSettings";
    private const string SONDAGEM_TOKEN_ISSUER_KEY = "SondagemTokenSettings:Issuer";
    private const string SONDAGEM_TOKEN_AUDIENCE_KEY = "SondagemTokenSettings:Audience";
    private const string SONDAGEM_TOKEN_SIGNING_KEY = "SondagemTokenSettings:IssuerSigningKey";
    private const int TOKEN_EXPIRATION_HOURS = 8;

    private readonly IConfiguration configuration;
    private readonly IAuthenticationService authenticationService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AutenticacaoUseCase(
        IConfiguration configuration,
        IAuthenticationService authenticationService,
        IHttpContextAccessor httpContextAccessor)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<TokenSondagemDto> Autenticar(string tokenSgp)
    {
        var sgpPrincipal = await AuthenticateSgpToken(tokenSgp);

        var userClaims = ExtractUserClaims(sgpPrincipal);

        var sondagemToken = GenerateSondagemToken(userClaims);

        return new TokenSondagemDto { ApiAToken = sondagemToken };
    }

    private async Task<ClaimsPrincipal> AuthenticateSgpToken(string tokenSgp)
    {
        var currentHttpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("Erro ao processar a autenticação: O contexto da requisição web não pôde ser obtido.");

        var tempHttpContext = new DefaultHttpContext
        {
            RequestServices = currentHttpContext.RequestServices
        };
        tempHttpContext.Request.Headers.Authorization = $"Bearer {tokenSgp}";

        var authenticateResult = await authenticationService.AuthenticateAsync(tempHttpContext, SGP_API_TOKEN_SCHEME);

        if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
        {
            throw new UnauthorizedAccessException("Falha na autenticação do token da API SGP. Token inválido ou expirado.");
        }

        return authenticateResult.Principal;
    }

    private static List<Claim> ExtractUserClaims(ClaimsPrincipal principal)
    {
        var login = principal.FindFirst("login")?.Value
                    ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var nome = principal.FindFirst("nome")?.Value
                   ?? principal.FindFirst(ClaimTypes.Name)?.Value;

        var rf = principal.FindFirst("rf")?.Value;
        var perfil = principal.FindFirst("perfil")?.Value;
        var roles = principal.FindAll("roles").Select(c => c.Value).ToList();

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(nome))
        {
            throw new InvalidOperationException("Não foi possível identificar o usuário: Claims essenciais (login ou nome) estão ausentes no token da API do SGP.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, login),
            new Claim(ClaimTypes.Name, nome),
            new Claim("rf", rf ?? string.Empty),
            new Claim("perfil", perfil ?? string.Empty)
        };

        if (roles.Count != 0)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        return claims;
    }

    private string GenerateSondagemToken(List<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var signingKey = configuration[SONDAGEM_TOKEN_SIGNING_KEY]
            ?? throw new InvalidOperationException($"A chave JWT para a API Sondagem não está configurada.");
        var key = Encoding.UTF8.GetBytes(signingKey);

        var issuer = configuration[SONDAGEM_TOKEN_ISSUER_KEY]
            ?? throw new InvalidOperationException($"O Issuer para a API Sondagem não está configurado.");
        var audience = configuration[SONDAGEM_TOKEN_AUDIENCE_KEY]
            ?? throw new InvalidOperationException($"O Audience para a API Sondagem não está configurado.");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(TOKEN_EXPIRATION_HOURS),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
