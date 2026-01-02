namespace SME.Sondagem.Infra.Dtos.Proficiencia;

public class ProficienciaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int ComponenteCurricularId { get; set; }
    public DateTime CriadoEm { get; set; }
    public string CriadoPor { get; set; } = string.Empty;
    public string CriadoRF { get; set; } = string.Empty;
    public DateTime? AlteradoEm { get; set; }
    public string? AlteradoPor { get; set; }
    public string? AlteradoRF { get; set; }
}