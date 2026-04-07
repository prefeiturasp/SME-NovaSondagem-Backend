namespace SME.Sondagem.Infrastructure.Dtos.Relatorio;

public class RelatorioConsolidadoSondagemDto
{
    public string Titulo { get; set; } = string.Empty;
    public IEnumerable<RelatorioConsolidadoAnoDto> Anos { get; set; } = new List<RelatorioConsolidadoAnoDto>();
}

public class RelatorioConsolidadoAnoDto
{
    public int Ano { get; set; }
    public IEnumerable<RelatorioConsolidadoRespostaDto> Respostas { get; set; } = new List<RelatorioConsolidadoRespostaDto>();
    public int TotalEstudantes { get; set; }
    public double PercentualTotal { get; set; }
}

public class RelatorioConsolidadoRespostaDto
{
    public string Resposta { get; set; } = string.Empty;
    public IEnumerable<RelatorioConsolidadoRacaDto> Racas { get; set; } = new List<RelatorioConsolidadoRacaDto>();
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
