namespace SME.Sondagem.Infrastructure.Dtos.Questionario;

public class ParametroSondagemQuestionarioCompletoDto
{
    public int Id { get; set; }
    public int IdQuestionario { get; set; }
    public string? Valor { get; set; }
    public int Tipo { get; set; }
}