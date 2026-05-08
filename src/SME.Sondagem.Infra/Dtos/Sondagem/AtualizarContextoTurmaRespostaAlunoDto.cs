namespace SME.Sondagem.Infrastructure.Dtos.Sondagem;

public class AtualizarContextoTurmaRespostaAlunoDto
{
    public int Id { get; set; }
    public string TurmaId { get; set; } = string.Empty;
    public string UeId { get; set; } = string.Empty;
    public string DreId { get; set; } = string.Empty;
    public int? AnoTurma { get; set; }
}
