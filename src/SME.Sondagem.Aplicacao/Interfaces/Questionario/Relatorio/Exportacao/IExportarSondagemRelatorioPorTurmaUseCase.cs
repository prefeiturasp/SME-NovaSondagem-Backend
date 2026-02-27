using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;

public interface IExportarSondagemRelatorioPorTurmaUseCase
{
    public Task ExportarSondagemRelatorio([FromQuery] FiltroRelatorio filtro, CancellationToken cancellationToken);
}
