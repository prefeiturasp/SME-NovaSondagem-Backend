using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

public class FiltroRelatorioConsolidado : FiltroConsolidadoDto, IFiltroRelatorioExportacaoSondagem
{
    public FormatoRelatorio ExtensaoRelatorio { get; set; }
}
