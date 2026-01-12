using SME.Sondagem.Dominio.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class QuestionarioDto
{
    
    [JsonPropertyName("codigoturma")]
    public int CodigoTurma { get; set; }
    
    [JsonPropertyName("nometurma")]
    public string NomeTurma { get; set; } = string.Empty;

    [JsonPropertyName("anoletivo")]
    public int AnoLetivo { get; set; }
    
    [JsonPropertyName("codigoescola")]
    public string CodigoEscola { get; set; } = string.Empty;

    public TipoQuestionario Tipo { get; set; }
}
