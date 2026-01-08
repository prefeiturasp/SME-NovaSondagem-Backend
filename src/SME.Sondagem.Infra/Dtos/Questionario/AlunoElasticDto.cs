using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class AlunoElasticDto
{
    [JsonPropertyName("codigoaluno")]
    public int CodigoAluno { get; set; }
    
    [JsonPropertyName("codigomatricula")]
    public int CodigoMatricula { get; set; }
    
    [JsonPropertyName("nomealuno")]
    public string NomeAluno { get; set; }
    
    [JsonPropertyName("nomesocialaluno")]
    public string? NomeSocialAluno { get; set; }
    
    [JsonPropertyName("numeroalunochamada")]
    public string NumeroAlunoChamada { get; set; }
    
    [JsonPropertyName("datanascimento")]
    public DateTime DataNascimento { get; set; }
    
    [JsonPropertyName("codigoturma")]
    public int CodigoTurma { get; set; }
    
    [JsonPropertyName("codigoescola")]
    public string CodigoEscola { get; set; }
    
    [JsonPropertyName("codigodre")]
    public string CodigoDre { get; set; }
    
    [JsonPropertyName("anoletivo")]
    public int AnoLetivo { get; set; }
    
    [JsonPropertyName("situacaomatricula")]
    public string SituacaoMatricula { get; set; }
    
    [JsonPropertyName("codigosituacaomatricula")]
    public int CodigoSituacaoMatricula { get; set; }
    
    [JsonPropertyName("datamatricula")]
    public DateTime DataMatricula { get; set; }
    
    [JsonPropertyName("datasituacao")]
    public DateTime DataSituacao { get; set; }
    
    [JsonPropertyName("datasituacaoaluno")]
    public DateTime DataSituacaoAluno { get; set; }
    
    [JsonPropertyName("sequencia")]
    public int Sequencia { get; set; }
    
    [JsonPropertyName("possuideficiencia")]
    public int PossuiDeficiencia { get; set; }
    
    [JsonPropertyName("nomeresponsavel")]
    public string NomeResponsavel { get; set; }
    
    [JsonPropertyName("tiporesponsavel")]
    public int TipoResponsavel { get; set; }
    
    [JsonPropertyName("celularresponsavel")]
    public string CelularResponsavel { get; set; }
    
    [JsonPropertyName("dataatualizacaocontato")]
    public DateTime DataAtualizacaoContato { get; set; }
}