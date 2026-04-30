using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;

public interface IExportarSondagemRelatorioConsolidadoRacaUseCase
{
    public Task Exportar(FiltroRelatorioConsolidado filtro, CancellationToken cancellationToken);
}
