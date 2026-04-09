namespace SME.Sondagem.Infrastructure.Dtos.Relatorio;

public class FiltroConsolidadoDto
{
    public int AnoLetivo { get; set; }
    public string? Dre { get; set; }
    public string? Ue { get; set; }
    public int Modalidade { get; set; }
    public int ProficienciaId { get; set; }
    public int ComponenteCurricularId { get; set; }
    public List<int>? AnoTurma { get; set; }
    public int? BimestreId { get; set; }
    public int? GeneroId { get; set; }
    public int? RacaId { get; set; }
    public bool? Pap { get; set; }
    public bool? Aee { get; set; }
    public bool? Deficiente { get; set; }
}
