using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Policies;

[ExcludeFromCodeCoverage]
public class PoliticaPolly
{
    protected PoliticaPolly() { }
    public static string PublicaFila => "RetryPolicyFilasRabbit";
}