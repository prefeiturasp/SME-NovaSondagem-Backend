namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class OpcaoQuestaoComplementar : EntidadeBase
{
    public OpcaoQuestaoComplementar(long opcaoRespostaId, long questaoComplementarId)
    {
        OpcaoRespostaId = opcaoRespostaId;
        QuestaoComplementarId = questaoComplementarId;
    }

    public long OpcaoRespostaId { get; private set; }
    public long QuestaoComplementarId { get; private set; }

    // Navegação
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
    public virtual Questao QuestaoComplementar { get; private set; } = null!;
}
