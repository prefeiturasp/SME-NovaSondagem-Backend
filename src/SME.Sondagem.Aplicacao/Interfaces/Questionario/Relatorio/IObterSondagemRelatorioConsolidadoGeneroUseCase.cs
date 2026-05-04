using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;

public interface IObterSondagemRelatorioConsolidadoGeneroUseCase
{
    public Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken);
}
