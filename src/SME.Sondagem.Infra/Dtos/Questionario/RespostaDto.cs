using SME.Sondagem.Infrastructure.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class RespostaDto 
{
    public int Id { get; set; }
    public int OpcaoRespostaId { get; set; }
}
