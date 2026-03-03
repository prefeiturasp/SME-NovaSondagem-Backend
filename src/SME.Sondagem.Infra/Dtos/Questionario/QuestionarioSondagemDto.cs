using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class QuestionarioSondagemDto
{
    public int QuestionarioId { get; set; }
    public int QuestaoId { get; set; }
    public int SondagemId { get; set; }
    public string TituloTabelaRespostas { get; set; } = string.Empty;
    public bool PodeSalvar { get; set; }
    public IEnumerable<EstudanteQuestionarioDto>? Estudantes { get; set; }
    public string? InseridoPor { get; set; }
    public string? AlteradoPor { get; set; }
}