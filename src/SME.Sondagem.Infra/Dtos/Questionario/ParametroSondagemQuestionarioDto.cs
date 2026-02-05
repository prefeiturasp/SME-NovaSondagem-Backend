using System.Text.Json.Serialization;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public class ParametroSondagemQuestionarioDto 
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
        public int Id { get; set; }
        public int IdQuestionario { get; set; }
        public int IdParametroSondagem { get; set; }
        public string? Valor { get; set; }
    }
}
