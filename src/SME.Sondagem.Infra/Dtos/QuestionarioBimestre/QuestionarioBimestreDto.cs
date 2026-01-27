namespace SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

public class QuestionarioBimestreDto : BaseDto
{
    public int QuestionarioId { get; set; }
    public int BimestreId { get; set; }
    public string? DescricaoBimestre { get; set; }
    public int? CodBimestreEnsinoEol { get; set; }
}