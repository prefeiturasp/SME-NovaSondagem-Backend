using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

public interface IFiltroRelatorioExportacaoSondagem
{
    FormatoRelatorio ExtensaoRelatorio { get; set; }
}
