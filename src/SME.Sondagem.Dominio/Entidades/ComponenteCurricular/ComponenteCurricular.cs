namespace SME.Sondagem.Dominio.Entidades.ComponenteCurricular;

public class ComponenteCurricular : EntidadeBase
{
    public ComponenteCurricular(string nome, int? ano, string? modalidade)
    {
        Nome = nome;
        Ano = ano;
        Modalidade = modalidade;
    }

    public string Nome { get; private set; }
    public int? Ano { get; private set; }
    public string? Modalidade { get; private set; }

    // Navegação
    public virtual ICollection<Proficiencia.Proficiencia> Proficiencias { get; private set; } = new List<Proficiencia.Proficiencia>();
    public virtual ICollection<Questionario.Questionario> Questionarios { get; private set; } = new List<Questionario.Questionario>();
}