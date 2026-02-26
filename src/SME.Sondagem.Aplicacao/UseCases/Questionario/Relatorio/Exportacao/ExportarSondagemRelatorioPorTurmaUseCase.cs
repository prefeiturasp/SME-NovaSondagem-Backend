using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioPorTurmaUseCase : IExportarSondagemRelatorioPorTurmaUseCase
{
    private readonly ISolicitacaoRelatorioService _solicitacaoRelatorioService;

    public ExportarSondagemRelatorioPorTurmaUseCase(ISolicitacaoRelatorioService solicitacaoRelatorioService)
    {
        _solicitacaoRelatorioService = solicitacaoRelatorioService;
    }

    public async Task ExportarSondagemRelatorio([FromQuery] FiltroRelatorio filtro, CancellationToken cancellationToken)
    {
        try
        {
            var possuiRelatorioSolicitado = await _solicitacaoRelatorioService.ObterSolicitacaoRelatorioAsync(filtro, cancellationToken);

            if (!possuiRelatorioSolicitado)
                await _solicitacaoRelatorioService.RegistrarSolicitacaoRelatorioAsync(filtro, cancellationToken);
        }
        catch (Exception ex)
        {
            throw ex;
        }


    }
}
