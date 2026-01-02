using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class OpcaoRespostaDto
{
    public string DescricaoOpcaoResposta { get; set; } = string.Empty;
    public string? Legenda { get; set; }
    public string? CorFundo { get; set; }
    public string? CorTexto { get; set; }
    public DateTime? AlteradoEm { get; set; }
    public string? AlteradoPor { get; set; }
    public string? AlteradoRF { get; set; }
    public DateTime CriadoEm { get; set; }
    public string CriadoPor { get; set; } = string.Empty;
    public string CriadoRF { get; set; } = string.Empty;
}
