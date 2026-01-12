using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class TurmaElasticDto
{
    [JsonPropertyName("codigoturma")]
    public int CodigoTurma { get; set; }
    
    [JsonPropertyName("codigoescola")]
    public string CodigoEscola { get; set; } = string.Empty;

    [JsonPropertyName("anoletivo")]
    public int AnoLetivo { get; set; }
    
    [JsonPropertyName("NomeTurma")]
    public string NomeTurma { get; set; } = string.Empty;

    [JsonPropertyName("SerieEnsino")]
    public string SerieEnsino { get; set; } = string.Empty;

    [JsonPropertyName("NomeFiltro")]
    public string NomeFiltro { get; set; } = string.Empty;

    [JsonPropertyName("Modalidade")]
    public int Modalidade { get; set; }
    
    [JsonPropertyName("AnoTurma")]
    public string AnoTurma { get; set; } = string.Empty;

    [JsonPropertyName("TipoTurma")]
    public int TipoTurma { get; set; }

    [JsonPropertyName("componentes")]
    public List<ComponenteCurricularElasticDto> Componentes { get; set; } = new();
}

[ExcludeFromCodeCoverage]
public class ComponenteCurricularElasticDto
{
    [JsonPropertyName("NomeComponenteCurricular")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("ComponenteCurricularCodigo")]
    public int Codigo { get; set; }
}