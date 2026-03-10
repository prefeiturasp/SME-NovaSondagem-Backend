using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades;

public class ModalidadeComponenteCurricular : EntidadeBase
{
    public Modalidade ModalidadeId { get; private set; }
    public int ComponenteCurricularId { get; private set; }
    public virtual ComponenteCurricular? ComponenteCurricular { get; private set; }
}