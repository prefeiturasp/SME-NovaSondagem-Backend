namespace SME.Sondagem.Infrastructure.Dtos.Relatorio;

public class RelatorioConsolidadoSondagemDto
{
    public string Titulo { get; set; } = string.Empty;
    public IEnumerable<RelatorioConsolidadoQuestaoDto> Questoes { get; set; } = new List<RelatorioConsolidadoQuestaoDto>();
}

public class RelatorioConsolidadoQuestaoDto
{
    public int QuestaoId { get; set; }
    public string QuestaoNome { get; set; } = string.Empty;
    public IEnumerable<RelatorioConsolidadoRespostaDto> Respostas { get; set; } = new List<RelatorioConsolidadoRespostaDto>();
    public IEnumerable<RelatorioConsolidadoGeneroDto> TotaisPorGenero { get; set; } = new List<RelatorioConsolidadoGeneroDto>();
    public IEnumerable<RelatorioConsolidadoRacaDto> TotaisPorRaca { get; set; } = new List<RelatorioConsolidadoRacaDto>();
    public int TotalEstudantes { get; set; }
    public double PercentualTotal { get; set; }
}

public class RelatorioConsolidadoRespostaDto
{
    public string Resposta { get; set; } = string.Empty;
    public IEnumerable<RelatorioConsolidadoRacaDto> Racas { get; set; } = new List<RelatorioConsolidadoRacaDto>();
    public IEnumerable<RelatorioConsolidadoGeneroDto> Generos { get; set; } = new List<RelatorioConsolidadoGeneroDto>();
    public IEnumerable<RelatorioConsolidadoGeneroRacaDto> GenerosComRacas { get; set; } = new List<RelatorioConsolidadoGeneroRacaDto>();
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
    public IEnumerable<RelatorioConsolidadoRacaDto> Racas { get; set; } = new List<RelatorioConsolidadoRacaDto>();
}
