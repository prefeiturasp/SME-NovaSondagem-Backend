using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades;

public class Proficiencia : EntidadeBase
{
    public Proficiencia(string nome, int componenteCurricularId, int  modalidadeId)
    {
        Nome = nome;
        ComponenteCurricularId = componenteCurricularId;
        ModalidadeId = modalidadeId;
    }

    public string Nome { get; private set; }
    public int ComponenteCurricularId { get; private set; }
    public int ModalidadeId { get; private set; }

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

    protected void AtualizarModalidade(int modalidadeId)
    {
        if (modalidadeId <= 0)
            throw new ArgumentException("Modalidade deve ser maior que zero.", nameof(modalidadeId));
        ModalidadeId = modalidadeId;
    }

    public void Atualizar(string nome, int componenteCurricularId ,int modalidadeId)
    {
        AtualizarNome(nome);
        AtualizarComponenteCurricular(componenteCurricularId);
        AtualizarModalidade(modalidadeId);
    }
}