using SME.Sondagem.Infra.Dtos.Questionario;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

[ExcludeFromCodeCoverage]
public class QuestionarioSondagemRelatorioDto
{
    public string TituloTabelaRespostas { get; set; } = string.Empty;
    public string Semestre {  get; set; } = string.Empty;
    public IEnumerable<EstudanteQuestionarioDto>? Estudantes { get; set; }
    public IEnumerable<LegendaQuestionarioDto>? Legenda { get; set; }
    public int? QuestionarioId { get; set; }
}