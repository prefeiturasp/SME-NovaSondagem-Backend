using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.EnvironmentVariables;

[ExcludeFromCodeCoverage]
public class ConnectionStringOptions
{
    public string? ApiSondagemExterna { get; set; }
    public string? ApiSondagem { get; set; }
    public string? ApiSondagemLeitura { get; set; }
}