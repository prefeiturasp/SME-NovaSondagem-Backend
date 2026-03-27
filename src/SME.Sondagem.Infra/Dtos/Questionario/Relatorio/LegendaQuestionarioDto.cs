using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

[ExcludeFromCodeCoverage]
public class LegendaQuestionarioDto
{
    public int Id { get; set; }
    public int Ordem { get; set; }
    public string DescricaoOpcaoResposta { get; set; } = string.Empty;
    public string? Legenda { get; set; }
    public string? CorFundo { get; set; }
    public string? CorTexto { get; set; }
}