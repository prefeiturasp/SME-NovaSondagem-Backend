namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;

using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

public interface IExportarSondagemRelatorioConsolidadoUseCase
{
    Task Exportar(FiltroRelatorioConsolidado filtro, CancellationToken cancellationToken);
}
