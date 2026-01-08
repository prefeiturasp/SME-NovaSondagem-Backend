using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class Questionario : EntidadeBase
{
    public required string Nome { get; init; }
    public required TipoQuestionario Tipo { get; init; }
    public required int AnoLetivo { get; init; }
    public required int ComponenteCurricularId { get; init; }
    public required int ProficienciaId { get; init; }
    public int? ModalidadeId { get; init; }
    public int? SerieAno { get; init; }

    // Navegação
    public virtual ComponenteCurricular ComponenteCurricular { get; private set; } = null!;
    public virtual Proficiencia Proficiencia { get; private set; } = null!;
    public virtual ICollection<Questao> Questoes { get; private set; } = new List<Questao>();
    public virtual ICollection<Sondagem.Sondagem> Sondagens { get; private set; } = new List<Sondagem.Sondagem>();
}