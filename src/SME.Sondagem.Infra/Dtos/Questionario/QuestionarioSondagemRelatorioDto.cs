using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class QuestionarioSondagemRelatorioDto
{
    public string TituloTabelaRespostas { get; set; } = string.Empty;
    public IEnumerable<EstudanteQuestionarioDto>? Estudantes { get; set; }
}