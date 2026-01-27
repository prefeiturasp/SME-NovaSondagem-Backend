namespace SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

public class VincularBimestresDto
{
    public int QuestionarioId { get; set; }
    public List<int> BimestreIds { get; set; } = new List<int>();
}