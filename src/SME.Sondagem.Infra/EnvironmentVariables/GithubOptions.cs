using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.EnvironmentVariables;

[ExcludeFromCodeCoverage]
public class GithubOptions
{
    public string? Url { get; set; }
    public string? RepositorioApi { get; set; }
    public string? RepositorioFront { get; set; }
}