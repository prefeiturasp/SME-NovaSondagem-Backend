using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.EnvironmentVariables;

[ExcludeFromCodeCoverage]
public class JwtOptions
{
    public static string Secao => "Jwt";
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? ExpiresInMinutes { get; set; }
    public string? IssuerSigningKey { get; set; }
}
