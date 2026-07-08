namespace SME.Sondagem.Infrastructure.Dtos.Sondagem;

public class LotePendenteAeeDto
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public string TurmaId { get; set; } = null!;
    public string? UeId { get; set; }
}
