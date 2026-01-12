using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class OpcaoQuestaoComplementarDto
{
    public long Id { get; set; }
    public long OpcaoRespostaId { get; set; }
    public OpcaoRespostaDto? OpcaoResposta { get; set; }
    public QuestaoDto? QuestaoComplementar { get; set; }
    public long QuestaoComplementarId { get; set; }
}
