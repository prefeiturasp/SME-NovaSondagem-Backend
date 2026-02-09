namespace SME.Sondagem.Dominio.Entidades.Sondagem;

public class Sondagem : EntidadeBase
{
    public Sondagem(string descricao, DateTime dataAplicacao)
    {
        Descricao = descricao;
        DataAplicacao = dataAplicacao;
    }

    public string Descricao { get; private set; }

    public DateTime DataAplicacao { get; private set; }

    // Navegação
    public virtual ICollection<Questionario.Questionario> Questionarios { get; private set; } = null!;
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();
    public virtual ICollection<SondagemPeriodoBimestre> PeriodosBimestre { get; private set; } = new List<SondagemPeriodoBimestre>();

    public void AtualizarDescricao(string descricao)
    {
        Descricao = descricao;
    }

    public void AtualizarDataAplicacao(DateTime dataAplicacao)
    {
        DataAplicacao = dataAplicacao;
    }
}