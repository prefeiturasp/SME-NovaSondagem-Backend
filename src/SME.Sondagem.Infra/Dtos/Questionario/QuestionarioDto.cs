using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Infra.Dtos.Questionario;

public class QuestionarioDto
{
    public long Id { get; set; }
    public string? Nome { get; set; }
    public TipoQuestionario Tipo { get; set; }
}
