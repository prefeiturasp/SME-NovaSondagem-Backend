using SME.Sondagem.Infrastructure.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class QuestaoOpcaoRespostaDto : BaseDto
{
    public int QuestaoId { get; set; }
    public int OpcaoRespostaId { get; set; }
    public int Ordem { get; set; }
}
