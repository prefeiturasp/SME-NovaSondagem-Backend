using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.EnvironmentVariables
{
    [ExcludeFromCodeCoverage]
    public class ElasticOptions
    {
        public static string Secao => "ElasticSearch";
        public string Urls { get; set; } = string.Empty;
        public string DefaultIndex { get; set; } = string.Empty;
        public string PrefixIndex { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}