namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public class ParametroSondagemQuestionarioDto : BaseDto
    {
        public int IdQuestionario { get; set; }
        public int IdParametroSondagem { get; set; }
        public string? Valor { get; set; }
    }
}
