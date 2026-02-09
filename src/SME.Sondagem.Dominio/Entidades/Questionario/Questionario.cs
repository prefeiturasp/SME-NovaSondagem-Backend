using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class Questionario : EntidadeBase
{
    public Questionario(
        string nome,
        TipoQuestionario tipo,
        int anoLetivo,
        int componenteCurricularId,
        int proficienciaId,
        int sondagemId,
        int? modalidadeId = null,
        int? serieAno = null)
    {
        ValidarEAplicar(
            nome,
            tipo,
            anoLetivo,
            componenteCurricularId,
            proficienciaId,
            sondagemId,
            modalidadeId,
            serieAno);
    }

    public string Nome { get; private set; } = string.Empty;
    public TipoQuestionario Tipo { get; private set; }
    public int AnoLetivo { get; private set; }
    public int ComponenteCurricularId { get; private set; }
    public int ProficienciaId { get; private set; }
    public int SondagemId { get; private set; }
    public int? ModalidadeId { get; private set; }
    public int? SerieAno { get; private set; }

    // Navegação
    public virtual Entidades.Sondagem.Sondagem Sondagem { get; private set; } = null!;
    public virtual ComponenteCurricular ComponenteCurricular { get; private set; } = null!;
    public virtual Proficiencia Proficiencia { get; private set; } = null!;
    public virtual ICollection<Questao> Questoes { get; private set; } = new List<Questao>();
    public virtual ICollection<QuestionarioBimestre> QuestionariosBimestres { get; private set; } = new List<QuestionarioBimestre>();

    public void Atualizar(
        string nome,
        TipoQuestionario tipo,
        int anoLetivo,
        int componenteCurricularId,
        int proficienciaId,
        int sondagemId,
        int? modalidadeId = null,
        int? serieAno = null)
    {
        ValidarEAplicar(
            nome,
            tipo,
            anoLetivo,
            componenteCurricularId,
            proficienciaId,
            sondagemId,
            modalidadeId,
            serieAno);
    }

    private void ValidarEAplicar(
        string nome,
        TipoQuestionario tipo,
        int anoLetivo,
        int componenteCurricularId,
        int proficienciaId,
        int sondagemId,
        int? modalidadeId,
        int? serieAno)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(nome));

        if (componenteCurricularId <= 0)
            throw new ArgumentException("Componente Curricular Id deve ser maior que zero.", nameof(componenteCurricularId));

        if (proficienciaId <= 0)
            throw new ArgumentException("Proficiencia Id deve ser maior que zero.", nameof(proficienciaId));

        if (sondagemId <= 0)
            throw new ArgumentException("Sondagem Id deve ser maior que zero.", nameof(sondagemId));

        Nome = nome;
        Tipo = tipo;
        AnoLetivo = anoLetivo;
        ComponenteCurricularId = componenteCurricularId;
        ProficienciaId = proficienciaId;
        SondagemId = sondagemId;
        ModalidadeId = modalidadeId;
        SerieAno = serieAno;
    }
}