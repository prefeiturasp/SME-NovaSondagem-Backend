using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dominio.Entidades;

public class Semestre : EntidadeBase
{
    protected Semestre() { }

    public Semestre(int codSemestreEol, string? descricao)
    {
        CodSemestreEol = codSemestreEol;
        Descricao = descricao;
    }

    public int CodSemestreEol { get; private set; }
    public string? Descricao { get; private set; }

    public virtual ICollection<RespostaAluno> RespostaAlunos { get; private set; } = new List<RespostaAluno>();
}
