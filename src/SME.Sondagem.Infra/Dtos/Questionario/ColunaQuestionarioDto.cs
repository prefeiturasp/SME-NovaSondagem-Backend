using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class ColunaQuestionarioDto
{
    public int IdCiclo { get; set; }
    public string DescricaoColuna { get; set; } = string.Empty;
    public bool PeriodoBimestreAtivo { get; set; }
    public int? QuestaoSubrespostaId { get; set; }
    public IEnumerable<OpcaoRespostaDto>? OpcaoResposta { get; set; }
    public RespostaDto? Resposta { get; set; }
}