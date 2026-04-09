namespace SME.Sondagem.Infrastructure.Dtos.Relatorio;

public class RelatorioRespostaAlunoDto
{
    public int Id { get; set; }
    
    public int SondagemId { get; set; }
    public string SondagemDescricao { get; set; } = string.Empty;
    
    public int AlunoId { get; set; }
    
    public int QuestaoId { get; set; }
    public string QuestaoNome { get; set; } = string.Empty;
    
    public int? OpcaoRespostaId { get; set; }
    public string? OpcaoRespostaDescricao { get; set; }
    public string? OpcaoRespostaLegenda { get; set; }
    
    public DateTime DataResposta { get; set; }
    
    public int? BimestreId { get; set; }
    public string? BimestreDescricao { get; set; }
    
    public string? TurmaId { get; set; }
    public string? UeId { get; set; }
    public string? DreId { get; set; }
    public int? AnoLetivo { get; set; }
    public int? AnoTurma { get; set; }
    public int? ModalidadeId { get; set; }
    
    public IEnumerable<RelatorioOpcaoRespostaDto>? OpcoesDisponiveis { get; set; }
    
    public RelatorioRacaCorDto? RacaCor { get; set; }
    public RelatorioGeneroSexoDto? GeneroSexo { get; set; }
    public RelatorioProgramaAtendimentoDto? ProgramaAtendimento { get; set; }
}

public class RelatorioOpcaoRespostaDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Legenda { get; set; }
    public int Ordem { get; set; }
    public string? CorFundo { get; set; }
    public string? CorTexto { get; set; }
}

public class RelatorioRacaCorDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int CodigoEol { get; set; }
}

public class RelatorioGeneroSexoDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Sigla { get; set; }
}

public class RelatorioProgramaAtendimentoDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
