using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Contexto;

[ExcludeFromCodeCoverage]
public class InternalClaim
{
    public string? Value { get; set; }
    public string? Type { get; set; }
}
