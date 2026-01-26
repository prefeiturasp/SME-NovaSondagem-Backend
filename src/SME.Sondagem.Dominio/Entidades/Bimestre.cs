using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dominio.Entidades;

public class Bimestre : EntidadeBase
{
    public Bimestre(int codBimestreEnsinoEol, string descricao)
    {
        CodBimestreEnsinoEol = codBimestreEnsinoEol;
        Descricao = descricao;
    }

    public int CodBimestreEnsinoEol { get; private set; }
    public string Descricao { get; private set; }

    // Navegação
    public virtual ICollection<Questionario.Questionario> Questionarios { get; private set; } = new List<Questionario.Questionario>();
    public virtual ICollection<SondagemPeriodoBimestre> PeriodosBimestre { get; private set; } = new List<SondagemPeriodoBimestre>();
    public virtual ICollection<RespostaAluno> RespostaAlunos { get; private set; } = new List<RespostaAluno>();
    public virtual ICollection<Questionario.QuestionarioBimestre> QuestionariosBimestres { get; private set; } = new List<Questionario.QuestionarioBimestre>(); // NOVA PROPRIEDADE

    public void AtualizarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição do bimestre não pode ser vazia.", nameof(descricao));

        Descricao = descricao;
    }

    public void AtualizarCodigoBimestreEnsino(int codBimestreEnsinoEol)
    {
        if (codBimestreEnsinoEol <= 0)
            throw new ArgumentException("Código do bimestre de ensino deve ser maior que zero.", nameof(codBimestreEnsinoEol));

        CodBimestreEnsinoEol = codBimestreEnsinoEol;
    }

    public void Atualizar(string descricao, int codBimestreEnsinoEol)
    {
        AtualizarDescricao(descricao);
        AtualizarCodigoBimestreEnsino(codBimestreEnsinoEol);
    }
}