using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class Questionario : EntidadeBase
{
    public string? Nome { get; set; }
    public TipoQuestionario Tipo { get; set; }
    public bool Excluido { get; set; }
}
