namespace SME.Sondagem.Infrastructure.Dtos.Sondagem;

public class AtualizarContextoRespostaAlunoDto
{
    public int Id { get; set; }
    public string TurmaId { get; set; } = string.Empty;
    public string UeId { get; set; } = string.Empty;
    public string DreId { get; set; } = string.Empty;
    public int AnoLetivo { get; set; }
    public int? AnoTurma { get; set; }
    public int? ModalidadeId { get; set; }
    public int? RacaCorId { get; set; }
    public int? GeneroSexoId { get; set; }
    public bool Pap { get; set; }
    public bool Aee { get; set; }
    public bool Deficiente { get; set; }
}
