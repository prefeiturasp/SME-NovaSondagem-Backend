using SME.Sondagem.Dominio.Enums;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class QuestionarioDto
{
    public long Id { get; set; }
    public string? Nome { get; set; }
    public TipoQuestionario Tipo { get; set; }
}
