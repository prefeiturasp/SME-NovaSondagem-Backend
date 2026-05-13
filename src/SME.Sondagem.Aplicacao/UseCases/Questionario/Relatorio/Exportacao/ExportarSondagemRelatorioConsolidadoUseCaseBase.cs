using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;
using SME.Sondagem.Infrastructure.Interfaces;
using System.Text.Json;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public abstract class ExportarSondagemRelatorioConsolidadoUseCaseBase<TFiltro>
    where TFiltro : IFiltroRelatorioExportacaoSondagem
{
    private readonly ISolicitacaoRelatorioService _solicitacaoRelatorioService;
    private readonly IServicoLog _servicoLog;
    private readonly IServicoMensageria _servicoMensageria;
    private readonly IServicoUsuario _servicoUsuario;

    protected ExportarSondagemRelatorioConsolidadoUseCaseBase(
        ISolicitacaoRelatorioService solicitacaoRelatorioService,
        IServicoLog servicoLog,
        IServicoMensageria servicoMensageria,
        IServicoUsuario servicoUsuario)
    {
        _solicitacaoRelatorioService = solicitacaoRelatorioService;
        _servicoLog = servicoLog;
        _servicoMensageria = servicoMensageria;
        _servicoUsuario = servicoUsuario;
    }

    protected abstract TipoRelatorio TipoRelatorio { get; }

    protected abstract string RotaRabbit { get; }

    protected virtual string ExchangeRabbitName => ExchangeRabbit.Sgp;

    protected virtual string NomeUseCase => GetType().Name;

    public async Task Exportar(TFiltro filtro, CancellationToken cancellationToken)
    {
        var codigoCorrelacao = Guid.NewGuid();
        var filtroSgp = MapearParaFiltroSgp(filtro, codigoCorrelacao);

        var solicitacaoRelatorioId = await RegistrarSolicitacaoRelatorio(filtroSgp, cancellationToken);

        await PublicarMensagemExportacao(filtro, solicitacaoRelatorioId, codigoCorrelacao);
    }

    private async Task<long> RegistrarSolicitacaoRelatorio(FiltroSolicitacaoRelatorioIntegracaoSgpDto filtro, CancellationToken ct)
    {
        return await _solicitacaoRelatorioService.RegistrarSolicitacaoRelatorioAsync(filtro, ct);
    }

    private async Task PublicarMensagemExportacao(TFiltro filtro, long solicitacaoRelatorioId, Guid codigoCorrelacao)
    {
        var mensagem = new MensagemRabbit(MapearParaFiltroRabbit(filtro, solicitacaoRelatorioId, codigoCorrelacao), codigoCorrelacao)
        {
            UsuarioLogadoRF = _servicoUsuario.ObterRFUsuarioLogado()
        };

        await _servicoMensageria.Publicar(mensagem, RotaRabbit, ExchangeRabbitName);
    }

    private FiltroSolicitacaoRelatorioIntegracaoRabbitDto MapearParaFiltroRabbit(TFiltro filtroRelatorio, long solicitacaoRelatorioId, Guid codigoCorrelacao)
    {
        return new FiltroSolicitacaoRelatorioIntegracaoRabbitDto
        {
            ExtensaoRelatorio = filtroRelatorio.ExtensaoRelatorio,
            TipoRelatorio = TipoRelatorio,
            UsuarioQueSolicitou = _servicoUsuario.ObterRFUsuarioLogado(),
            FiltrosUsados = filtroRelatorio,
            StatusSolicitacao = StatusSolicitacao.Solicitado,
            SolicitacaoRelatorioId = solicitacaoRelatorioId,
            CodigoCorrelacao = codigoCorrelacao
        };
    }

    private FiltroSolicitacaoRelatorioIntegracaoSgpDto MapearParaFiltroSgp(TFiltro filtroRelatorio, Guid codigoCorrelacao)
    {
        return new FiltroSolicitacaoRelatorioIntegracaoSgpDto
        {
            ExtensaoRelatorio = filtroRelatorio.ExtensaoRelatorio,
            TipoRelatorio = TipoRelatorio,
            UsuarioQueSolicitou = _servicoUsuario.ObterRFUsuarioLogado(),
            FiltrosUsados = JsonSerializer.Serialize(filtroRelatorio),
            StatusSolicitacao = StatusSolicitacao.Solicitado,
            CodigoCorrelacao = codigoCorrelacao
        };
    }
}

public abstract class ExportarSondagemRelatorioConsolidadoUseCaseBase
    : ExportarSondagemRelatorioConsolidadoUseCaseBase<FiltroRelatorioConsolidado>,
      IExportarSondagemRelatorioConsolidadoUseCase
{
    protected ExportarSondagemRelatorioConsolidadoUseCaseBase(
        ISolicitacaoRelatorioService solicitacaoRelatorioService,
        IServicoLog servicoLog,
        IServicoMensageria servicoMensageria,
        IServicoUsuario servicoUsuario)
        : base(solicitacaoRelatorioService, servicoLog, servicoMensageria, servicoUsuario)
    {
    }
}
