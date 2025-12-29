using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.EnvironmentVariables;

[ExcludeFromCodeCoverage]
public class RabbitLogOptions
{
    public const string Secao = "ConfiguracaoRabbitLog";
    public string? HostName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? VirtualHost { get; set; }
}