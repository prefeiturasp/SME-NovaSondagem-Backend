namespace SME.Sondagem.Infrastructure.Dtos.Relatorio;

public class FiltroConsolidadoDto
{
    public int AnoLetivo { get; set; }
    public string? Dre { get; set; }
    public string? Ue { get; set; }
    public int Modalidade { get; set; }
    public int ProficienciaId { get; set; }
    public int ComponenteCurricularId { get; set; }
    public int AnoTurma { get; set; }
    public int SemestreId { get; set; }
    public int? BimestreId { get; set; }
}
