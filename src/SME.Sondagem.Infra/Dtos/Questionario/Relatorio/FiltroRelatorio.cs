using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

[ExcludeFromCodeCoverage]
public class FiltroRelatorio : FiltroQuestionario
{
    public FormatoRelatorio ExtensaoRelatorio { get; set; }
}
