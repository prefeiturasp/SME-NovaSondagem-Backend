using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class OpcaoResposta : EntidadeBase
{
    public OpcaoResposta(int ordem, string descricaoOpcaoResposta, string? legenda, string? corFundo, string? corTexto)
    {
        Ordem = ordem;
        DescricaoOpcaoResposta = descricaoOpcaoResposta;
        Legenda = legenda;
        CorFundo = corFundo;
        CorTexto = corTexto;
    }

    public int Ordem { get; private set; }
    public string DescricaoOpcaoResposta { get; private set; }
    public string? Legenda { get; private set; }
    public string? CorFundo { get; private set; }
    public string? CorTexto { get; private set; }


    // Navegação
    public virtual ICollection<QuestaoOpcaoResposta> QuestaoOpcoes { get; private set; } = new List<QuestaoOpcaoResposta>();
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();

    public void Atualizar(int ordem, string descricaoOpcaoResposta, string? legenda, string? corFundo, string? corTexto)
    {
        Ordem = ordem;
        DescricaoOpcaoResposta = descricaoOpcaoResposta;
        Legenda = legenda;
        CorFundo = corFundo;
        CorTexto = corTexto;
    }
}