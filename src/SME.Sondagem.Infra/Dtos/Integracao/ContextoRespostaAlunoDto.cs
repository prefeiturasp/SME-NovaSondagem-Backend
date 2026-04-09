namespace SME.Sondagem.Infrastructure.Dtos.Integracao;

public class ContextoRespostaAlunoDto
{
    public int RespostaId { get; set; }
    public string? TurmaId { get; set; }
    public string? UeId { get; set; }
    public string? DreId { get; set; }
    public int? AnoLetivo { get; set; }
    public int? ModalidadeId { get; set; }
    public int? AnoTurma { get; set; }
    public int? GeneroId { get; set; }
    public int? RacaId { get; set; }
    public bool Pap { get; set; }
    public bool Aee { get; set; }
    public bool Deficiente { get; set; }
}
