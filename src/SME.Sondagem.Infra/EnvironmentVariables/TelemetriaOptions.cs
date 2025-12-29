using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.EnvironmentVariables;

[ExcludeFromCodeCoverage]
public class TelemetriaOptions
{
    public const string Secao = "Telemetria";
    public bool ApplicationInsights { get; set; }
    public bool Apm { get; set; }
}