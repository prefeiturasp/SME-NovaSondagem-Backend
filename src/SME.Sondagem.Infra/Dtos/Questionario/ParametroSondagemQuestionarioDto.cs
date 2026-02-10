namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public class ParametroSondagemQuestionarioDto 
    {
        public int Id { get; set; }
        public int IdQuestionario { get; set; }
        public int IdParametroSondagem { get; set; }
        public string? Valor { get; set; }
    }
}
