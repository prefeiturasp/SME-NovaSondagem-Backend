using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;

public interface IObterSondagemRelatorioConsolidadoUseCase
{
    public Task<QuestionarioSondagemRelatorioDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken);
}
