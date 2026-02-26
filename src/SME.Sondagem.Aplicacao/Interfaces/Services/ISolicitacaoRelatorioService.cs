using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Services;

public interface ISolicitacaoRelatorioService
{
    Task<bool> ObterSolicitacaoRelatorioAsync(FiltroRelatorio filtroRelatorio, CancellationToken cancellationToken = default);
    Task<bool> RegistrarSolicitacaoRelatorioAsync(FiltroRelatorio filtroRelatorio, CancellationToken cancellationToken = default);
}
