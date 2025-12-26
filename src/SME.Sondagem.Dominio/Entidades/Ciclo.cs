namespace SME.Sondagem.Dominio.Entidades;

public class Ciclo : EntidadeBase
{
    public Ciclo(int codCicloEnsinoEol, string descCiclo)
    {
        CodCicloEnsinoEol = codCicloEnsinoEol;
        DescCiclo = descCiclo;
    }

    public int CodCicloEnsinoEol { get; private set; }
    public string DescCiclo { get; private set; } = string.Empty;

    // Navegação
    public virtual ICollection<Questionario.Questionario> Questionarios { get; private set; } = new List<Questionario.Questionario>();
}