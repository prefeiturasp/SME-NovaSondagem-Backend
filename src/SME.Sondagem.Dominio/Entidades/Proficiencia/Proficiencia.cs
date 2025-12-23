namespace SME.Sondagem.Dominio.Entidades.Proficiencia;

public class Proficiencia : EntidadeBase
{
    public Proficiencia(string nome, int componenteCurricularId)
    {
        Nome = nome;
        ComponenteCurricularId = componenteCurricularId;
    }

    public string Nome { get; private set; }
    public int ComponenteCurricularId { get; private set; }

    // Navegação
    public virtual ComponenteCurricular.ComponenteCurricular ComponenteCurricular { get; private set; } = null!;
    public virtual ICollection<Questionario.Questionario> Questionarios { get; private set; } = new List<Questionario.Questionario>();
}