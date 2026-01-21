using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Sondagem;

[ExcludeFromCodeCoverage]
public class SondagemDto : BaseDto
{
    public string Descricao { get; set; } = String.Empty;
    public DateTime DataAplicacao { get; set; }
}
