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

    public void AtualizarDescricao(string descCiclo)
    {
        if (string.IsNullOrWhiteSpace(descCiclo))
            throw new ArgumentException("Descrição do ciclo não pode ser vazia.", nameof(descCiclo));
        
        DescCiclo = descCiclo;
    }

    public void AtualizarCodigoCicloEnsino(int codCicloEnsinoEol)
    {
        if (codCicloEnsinoEol <= 0)
            throw new ArgumentException("Código do ciclo de ensino deve ser maior que zero.", nameof(codCicloEnsinoEol));
        
        CodCicloEnsinoEol = codCicloEnsinoEol;
    }

    public void Atualizar(string descCiclo, int codCicloEnsinoEol)
    {
        AtualizarDescricao(descCiclo);
        AtualizarCodigoCicloEnsino(codCicloEnsinoEol);
    }
}