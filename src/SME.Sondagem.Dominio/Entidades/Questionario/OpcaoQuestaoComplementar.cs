namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class OpcaoQuestaoComplementar : EntidadeBase
{
    public long OpcaoRespostaId { get; set; }
    public OpcaoResposta OpcaoResposta { get; set; }
    public Questao QuestaoComplementar { get; set; }
    public long QuestaoComplementarId { get; set; }
}
