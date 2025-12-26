using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class OpcaoResposta : EntidadeBase
{
    public OpcaoResposta(string descricaoOpcaoResposta, string? legenda, string? corFundo, string? corTexto)
    {
        DescricaoOpcaoResposta = descricaoOpcaoResposta;
        Legenda = legenda;
        CorFundo = corFundo;
        CorTexto = corTexto;
    }

    public string DescricaoOpcaoResposta { get; private set; } = string.Empty;
    public string? Legenda { get; private set; }
    public string? CorFundo { get; private set; }
    public string? CorTexto { get; private set; }


    // Navegação
    public virtual ICollection<QuestaoOpcaoResposta> QuestaoOpcoes { get; private set; } = new List<QuestaoOpcaoResposta>();
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();
}