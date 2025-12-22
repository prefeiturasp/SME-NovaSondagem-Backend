namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class OpcaoResposta : EntidadeBase
{
    public OpcaoResposta(string descricaoOpcaoResposta, string? legenda)
    {
        DescricaoOpcaoResposta = descricaoOpcaoResposta;
        Legenda = legenda;
    }

    public string DescricaoOpcaoResposta { get; private set; } = string.Empty;
    public string? Legenda { get; private set; }

    // Navegação
    public virtual ICollection<QuestaoOpcaoResposta> QuestaoOpcoes { get; private set; } = new List<QuestaoOpcaoResposta>();
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();
}