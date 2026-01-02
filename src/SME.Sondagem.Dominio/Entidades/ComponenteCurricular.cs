namespace SME.Sondagem.Dominio.Entidades;

public class ComponenteCurricular : EntidadeBase
{
    public ComponenteCurricular(string nome, int? ano, string? modalidade, int codigoEol)
    {
        Nome = nome;
        Ano = ano;
        Modalidade = modalidade;
        CodigoEol = codigoEol;
    }

    public string Nome { get; private set; }
    public int? Ano { get; private set; }
    public string? Modalidade { get; private set; }
    public int CodigoEol { get; private set; }

    // Navegação
    public virtual ICollection<Proficiencia> Proficiencias { get; private set; } = new List<Proficiencia>();
    public virtual ICollection<Questionario.Questionario> Questionarios { get; private set; } = new List<Questionario.Questionario>();
}