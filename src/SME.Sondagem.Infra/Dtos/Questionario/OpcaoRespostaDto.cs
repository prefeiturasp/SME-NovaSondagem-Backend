using SME.Sondagem.Infrastructure.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class OpcaoRespostaDto : BaseDto
{
    public int Ordem { get; set; }
    public string DescricaoOpcaoResposta { get; set; } = string.Empty;
    public string? Legenda { get; set; }
    public string? CorFundo { get; set; }
    public string? CorTexto { get; set; }
}
