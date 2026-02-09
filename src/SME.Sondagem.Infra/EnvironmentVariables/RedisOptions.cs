using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.EnvironmentVariables;

[ExcludeFromCodeCoverage]
public class RedisOptions
{
    public static string Secao => "Redis";
    public string Endpoint { get; set; } = String.Empty;
    public int SyncTimeout { get; set; } = 5000;
    public Proxy Proxy { get; set; }
}