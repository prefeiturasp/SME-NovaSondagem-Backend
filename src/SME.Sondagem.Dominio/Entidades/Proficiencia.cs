namespace SME.Sondagem.Dominio.Entidades;

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
    public virtual ComponenteCurricular ComponenteCurricular { get; private set; } = null!;
    public virtual ICollection<Questionario.Questionario> Questionarios { get; private set; } = new List<Questionario.Questionario>();

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(nome));
        
        Nome = nome;
    }

    public void AtualizarComponenteCurricular(int componenteCurricularId)
    {
        if (componenteCurricularId <= 0)
            throw new ArgumentException("ComponenteCurricularId deve ser maior que zero.", nameof(componenteCurricularId));
        
        ComponenteCurricularId = componenteCurricularId;
    }

    public void Atualizar(string nome, int componenteCurricularId)
    {
        AtualizarNome(nome);
        AtualizarComponenteCurricular(componenteCurricularId);
    }
}