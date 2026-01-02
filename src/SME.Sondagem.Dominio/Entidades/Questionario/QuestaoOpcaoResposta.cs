namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class QuestaoOpcaoResposta : EntidadeBase
{
    public QuestaoOpcaoResposta(int questaoId, int opcaoRespostaId, int ordem)
    {
        QuestaoId = questaoId;
        OpcaoRespostaId = opcaoRespostaId;
        Ordem = ordem;
    }

    public int QuestaoId { get; private set; }
    public int OpcaoRespostaId { get; private set; }
    public int Ordem { get; private set; }

    // Navegação
    public virtual Questao Questao { get; private set; } = null!;
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
}