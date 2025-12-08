namespace SME.Sondagem.Infra.Dtos.Questionario;

public class OpcaoRespostaDto
{
    public OpcaoRespostaDto()
    {
        QuestoesComplementares = new List<OpcaoQuestaoComplementarDto>();
    }

    public QuestaoDto Questao { get; set; }
    public long QuestaoId { get; set; }
    public int Ordem { get; set; }
    public string Nome { get; set; }

    public string Observacao { get; set; }
    public bool Excluido { get; set; }
    public List<OpcaoQuestaoComplementarDto> QuestoesComplementares { get; set; }
}
