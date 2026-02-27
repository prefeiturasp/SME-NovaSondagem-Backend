using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;

namespace SME.Sondagem.Aplicacao.Interfaces.Services;

public interface ISolicitacaoRelatorioService
{
    Task<long> ObterSolicitacaoRelatorioAsync(FiltroSolicitacaoRelatorioIntegracaoSgpDto filtroRelatorio, CancellationToken cancellationToken = default);
    Task<long> RegistrarSolicitacaoRelatorioAsync(FiltroSolicitacaoRelatorioIntegracaoSgpDto filtroRelatorio, CancellationToken cancellationToken = default);
}
