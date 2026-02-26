using Microsoft.AspNetCore.Mvc;
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

    public async Task ExportarSondagemRelatorio([FromQuery] FiltroRelatorio filtroRelatorio, CancellationToken cancellationToken)
    {
        var filtro = CriarFiltroSolicitacaoRelatorioIntegracaoSgpDto(filtroRelatorio);
        try
        {
            var possuiRelatorioSolicitado = await _solicitacaoRelatorioService.ObterSolicitacaoRelatorioAsync(filtro, cancellationToken);

            if (possuiRelatorioSolicitado)
                return;

            await _solicitacaoRelatorioService.RegistrarSolicitacaoRelatorioAsync(filtro, cancellationToken);
        }
        catch (Exception ex)
        {
            _servicoLog.Registrar($"ExportarSondagemRelatorioPorTurmaUseCase - Erro ao verificar controle de acesso no relatório para o filtro: {filtro}", ex);
        }

        var menssagemRabbit = new MensagemRabbit(CriarFiltroSolicitacaoRelatorioIntegracaoRabbitDto(filtroRelatorio), Guid.NewGuid())
        {
            Action = RotasRabbit.RelatorioSondagemPorTurmaAction,
            RotaErro = RotasRabbit.RelatorioSondagemPorTurmaError,
            UsuarioLogadoRF = _servicoUsuario.ObterRFUsuarioLogado()
        };

        await _servicoMensageria.Publicar(menssagemRabbit, RotasRabbit.RelatorioSondagemPorTurma, ExchangeRabbit.WorkerRelatorios);
    }

    private FiltroSolicitacaoRelatorioIntegracaoRabbitDto CriarFiltroSolicitacaoRelatorioIntegracaoRabbitDto(FiltroRelatorio filtroRelatorio)
    {
        return new FiltroSolicitacaoRelatorioIntegracaoRabbitDto
        {
            ExtensaoRelatorio = filtroRelatorio.ExtensaoRelatorio,
            TipoRelatorio = TipoRelatorio.SondagemPorTurma,
            UsuarioQueSolicitou = _servicoUsuario.ObterRFUsuarioLogado(),
            FiltrosUsados = filtroRelatorio,
            StatusSolicitacao = StatusSolicitacao.Solicitado
        };
    }

    private FiltroSolicitacaoRelatorioIntegracaoSgpDto CriarFiltroSolicitacaoRelatorioIntegracaoSgpDto(FiltroRelatorio filtroRelatorio)
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
