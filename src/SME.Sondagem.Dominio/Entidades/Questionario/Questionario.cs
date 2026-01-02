using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class Questionario : EntidadeBase
{
    public required string Nome { get; init; }
    public required TipoQuestionario Tipo { get; init; }
    public required int AnoLetivo { get; init; }
    public required int ComponenteCurricularId { get; init; }
    public required int ProficienciaId { get; init; }
    public required int CicloId { get; init; }
    public int? ModalidadeId { get; init; }
    public string? ModalidadeDesc { get; init; }
    public int? DreId { get; init; }
    public string? DreNome { get; init; }
    public int? UeId { get; init; }
    public string? UeNome { get; init; }
    public int? SerieAno { get; init; }
    public string? SerieAnoNome { get; init; }
    public int? TurmaId { get; init; }
    public string? TurmaNome { get; init; }

    // Navegação
    public virtual ComponenteCurricular ComponenteCurricular { get; private set; } = null!;
    public virtual Proficiencia Proficiencia { get; private set; } = null!;
    public virtual Ciclo Ciclo { get; private set; } = null!;
    public virtual ICollection<Questao> Questoes { get; private set; } = new List<Questao>();
    public virtual ICollection<Sondagem.Sondagem> Sondagens { get; private set; } = new List<Sondagem.Sondagem>();
}