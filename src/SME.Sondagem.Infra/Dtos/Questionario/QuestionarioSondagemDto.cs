using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class QuestionarioSondagemDto
{
    public string TituloTabelaRespostas { get; set; }
    public IEnumerable<EstudanteQuestionarioDto> Estudantes { get; set; }
}