using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;

namespace SME.Sondagem.Aplicacao.Interfaces.Services;

public interface ISolicitacaoRelatorioService
{
    Task<bool> ObterSolicitacaoRelatorioAsync(FiltroSolicitacaoRelatorioIntegracaoSgpDto filtroRelatorio, CancellationToken cancellationToken = default);
    Task<bool> RegistrarSolicitacaoRelatorioAsync(FiltroSolicitacaoRelatorioIntegracaoSgpDto filtroRelatorio, CancellationToken cancellationToken = default);
}
