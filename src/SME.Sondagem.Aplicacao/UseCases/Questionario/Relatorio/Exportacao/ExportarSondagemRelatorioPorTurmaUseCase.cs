using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioPorTurmaUseCase : IExportarSondagemRelatorioPorTurmaUseCase
{
    private readonly ISolicitacaoRelatorioService _solicitacaoRelatorioService;
    private readonly IServicoLog _servicoLog;
    private readonly IServicoMensageria _servicoMensageria;
    private readonly IServicoUsuario _servicoUsuario;

    public ExportarSondagemRelatorioPorTurmaUseCase(ISolicitacaoRelatorioService solicitacaoRelatorioService, IServicoLog servicoLog, IServicoMensageria servicoMensageria, IServicoUsuario servicoUsuario)
    {
        _solicitacaoRelatorioService = solicitacaoRelatorioService;
        _servicoLog = servicoLog;
        _servicoMensageria = servicoMensageria;
        _servicoUsuario = servicoUsuario;
    }

    public async Task ExportarSondagemRelatorio(FiltroRelatorio filtro, CancellationToken cancellationToken)
    {
        var filtroSgp = MapearParaFiltroSgp(filtro);

        var (jaSolicitado, solicitacaoRelatorioId) = await RelatorioJaSolicitado(filtroSgp, cancellationToken);

        if (jaSolicitado)
            return;

        await PublicarMensagemExportacao(filtro, solicitacaoRelatorioId);
    }

    private async Task<(bool, long)> RelatorioJaSolicitado(FiltroSolicitacaoRelatorioIntegracaoSgpDto filtro, CancellationToken ct)
    {
        long solicitacaoRelatorioId = 0;

        try
        {
            solicitacaoRelatorioId = await _solicitacaoRelatorioService.ObterSolicitacaoRelatorioAsync(filtro, ct);
            if (solicitacaoRelatorioId != 0) return (true, solicitacaoRelatorioId);

            solicitacaoRelatorioId = await _solicitacaoRelatorioService.RegistrarSolicitacaoRelatorioAsync(filtro, ct);
            return (false, solicitacaoRelatorioId);
        }
        catch (Exception ex)
        {
            _servicoLog.Registrar($"ExportarSondagemRelatorioPorTurmaUseCase - Falha ao controlar duplicidade de relatório: {filtro.TipoRelatorio}", ex);
        }

        return (false, solicitacaoRelatorioId);
    }

    private async Task PublicarMensagemExportacao(FiltroRelatorio filtro, long solicitacaoRelatorioId)
    {
        var mensagem = new MensagemRabbit(MapearParaFiltroRabbit(filtro, solicitacaoRelatorioId), Guid.NewGuid())
        {
            Action = RotasRabbit.RelatorioSondagemPorTurmaAction,
            RotaErro = RotasRabbit.RelatorioSondagemPorTurmaError,
            UsuarioLogadoRF = _servicoUsuario.ObterRFUsuarioLogado()
        };

        await _servicoMensageria.Publicar(mensagem, RotasRabbit.RelatorioSondagemPorTurma, ExchangeRabbit.WorkerRelatorios);
    }

    private FiltroSolicitacaoRelatorioIntegracaoRabbitDto MapearParaFiltroRabbit(FiltroRelatorio filtroRelatorio, long solicitacaoRelatorioId)
    {
        return new FiltroSolicitacaoRelatorioIntegracaoRabbitDto
        {
            ExtensaoRelatorio = filtroRelatorio.ExtensaoRelatorio,
            TipoRelatorio = TipoRelatorio.SondagemPorTurma,
            UsuarioQueSolicitou = _servicoUsuario.ObterRFUsuarioLogado(),
            FiltrosUsados = filtroRelatorio,
            StatusSolicitacao = StatusSolicitacao.Solicitado,
            SolicitacaoRelatorioId = solicitacaoRelatorioId
        };
    }

    private FiltroSolicitacaoRelatorioIntegracaoSgpDto MapearParaFiltroSgp(FiltroRelatorio filtroRelatorio)
    {
        return new FiltroSolicitacaoRelatorioIntegracaoSgpDto
        {
            ExtensaoRelatorio = filtroRelatorio.ExtensaoRelatorio,
            TipoRelatorio = TipoRelatorio.SondagemPorTurma,
            UsuarioQueSolicitou = _servicoUsuario.ObterRFUsuarioLogado(),
            FiltrosUsados = JsonConvert.SerializeObject(filtroRelatorio),
            StatusSolicitacao = StatusSolicitacao.Solicitado
        };
    }
}
