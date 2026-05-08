namespace SME.Sondagem.Infrastructure.Dtos.Sondagem;

public class RespostaAlunoAnoTurmaDivergenteDto
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public string? TurmaId { get; set; }
    public int? AnoTurmaAtual { get; set; }
    public string SerieAnoCorreto { get; set; } = string.Empty;
    public string QuestaoNome { get; set; } = string.Empty;
    public int AnoLetivo { get; set; }
}
