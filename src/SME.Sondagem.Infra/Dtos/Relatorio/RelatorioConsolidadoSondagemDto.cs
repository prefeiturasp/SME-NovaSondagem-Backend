using System.Text.Json.Serialization;

namespace SME.Sondagem.Infrastructure.Dtos.Relatorio;

public class RelatorioConsolidadoSondagemDto
{
    public string Titulo { get; set; } = string.Empty;
    public IEnumerable<RelatorioConsolidadoQuestaoDto> Questoes { get; set; } = [];
}

public class RelatorioConsolidadoQuestaoDto
{
    public int QuestaoId { get; set; }
    public string QuestaoNome { get; set; } = string.Empty;
    public IEnumerable<RelatorioConsolidadoRespostaDto>? Respostas { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<RelatorioConsolidadoGeneroDto>? TotaisPorGenero { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<RelatorioConsolidadoRacaDto>? TotaisPorRaca { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<RelatorioConsolidadoAnoTurmaDto>? TotaisPorAnoTurma { get; set; }

    public int TotalEstudantes { get; set; }
    public double PercentualTotal { get; set; }
}

public class RelatorioConsolidadoRespostaDto
{
    public string Resposta { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<RelatorioConsolidadoRacaDto>? Racas { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<RelatorioConsolidadoGeneroDto>? Generos { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<RelatorioConsolidadoGeneroRacaDto>? GenerosComRacas { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<RelatorioConsolidadoAnoTurmaDto>? AnosTurma { get; set; }

    public int Total { get; set; }
    public double Percentual { get; set; }
    public int Ordem { get; set; }
    public string? CorFundo { get; set; }
    public string? CorTexto { get; set; }
}

public class RelatorioConsolidadoRacaDto
{
    public string Raca { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public double Percentual { get; set; }
}

public class RelatorioConsolidadoGeneroDto
{
    public string Genero { get; set; } = string.Empty;
    public string? Sigla { get; set; }
    public int Quantidade { get; set; }
    public double Percentual { get; set; }
}

public class RelatorioConsolidadoGeneroRacaDto
{
    public string Genero { get; set; } = string.Empty;
    public string? Sigla { get; set; }
    public int TotalGenero { get; set; }
    public double PercentualGenero { get; set; }
    public IEnumerable<RelatorioConsolidadoRacaDto>? Racas { get; set; }
}

public class RelatorioConsolidadoAnoTurmaDto
{
    public int Quantidade { get; set; }
    public double Percentual { get; set; }
}
