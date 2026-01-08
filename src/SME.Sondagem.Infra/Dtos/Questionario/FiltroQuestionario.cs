using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class FiltroQuestionario
{
    public int TurmaId { get; set; }
    public int ProficienciaId { get; set; }
    public int ComponenteCurricularId { get; set; }
    public int Modalidade { get; set; }
    public int Ano { get; set; }

    //será usado futuramente
    public int? CicloId { get; set; }
}
