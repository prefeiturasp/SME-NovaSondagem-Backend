using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class Questionario : EntidadeBase
{
    public Questionario(string nome, TipoQuestionario tipo, int anoLetivo, int? modalidadeId, string? modalidadeDesc, int? dreId, string? dreNome, int? ueId, string? ueNome, int? serieAno, string? serieAnoNome, int? turmaId, string? turmaNome, int componenteCurricularId, int proficienciaId, int cicloId)
    {
        Nome = nome;
        Tipo = tipo;
        AnoLetivo = anoLetivo;
        ModalidadeId = modalidadeId;
        ModalidadeDesc = modalidadeDesc;
        DreId = dreId;
        DreNome = dreNome;
        UeId = ueId;
        UeNome = ueNome;
        SerieAno = serieAno;
        SerieAnoNome = serieAnoNome;
        TurmaId = turmaId;
        TurmaNome = turmaNome;
        ComponenteCurricularId = componenteCurricularId;
        ProficienciaId = proficienciaId;
        CicloId = cicloId;
    }

    public string Nome { get; private set; } = string.Empty;
    public TipoQuestionario Tipo { get; private set; }
    public int AnoLetivo { get; private set; }
    public int? ModalidadeId { get; private set; }
    public string? ModalidadeDesc { get; private set; }
    public int? DreId { get; private set; }
    public string? DreNome { get; private set; }
    public int? UeId { get; private set; }
    public string? UeNome { get; private set; }
    public int? SerieAno { get; private set; }
    public string? SerieAnoNome { get; private set; }
    public int? TurmaId { get; private set; }
    public string? TurmaNome { get; private set; }
    public int ComponenteCurricularId { get; private set; }
    public int ProficienciaId { get; private set; }
    public int CicloId { get; private set; }

    // Navegação
    public virtual ComponenteCurricular.ComponenteCurricular ComponenteCurricular { get; private set; } = null!;
    public virtual Proficiencia.Proficiencia Proficiencia { get; private set; } = null!;
    public virtual Ciclo.Ciclo Ciclo { get; private set; } = null!;
    public virtual ICollection<Questao> Questoes { get; private set; } = new List<Questao>();
    public virtual ICollection<Sondagem.Sondagem> Sondagens { get; private set; } = new List<Sondagem.Sondagem>();
}