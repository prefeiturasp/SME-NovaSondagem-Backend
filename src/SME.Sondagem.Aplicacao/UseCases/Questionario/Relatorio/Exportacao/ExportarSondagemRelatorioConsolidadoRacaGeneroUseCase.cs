using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioConsolidadoRacaGeneroUseCase : ExportarSondagemRelatorioConsolidadoUseCaseBase, IExportarSondagemRelatorioConsolidadoRacaGeneroUseCase
{
    public ExportarSondagemRelatorioConsolidadoRacaGeneroUseCase(
        ISolicitacaoRelatorioService solicitacaoRelatorioService,
        IServicoLog servicoLog,
        IServicoMensageria servicoMensageria,
        IServicoUsuario servicoUsuario)
        : base(solicitacaoRelatorioService, servicoLog, servicoMensageria, servicoUsuario)
    {
    }

    protected override TipoRelatorio TipoRelatorio => TipoRelatorio.ConsolidadoPorRacaGenero;

    protected override string RotaRabbit => RotasRabbit.RelatorioSondagemConsolidadoPorRacaGenero;
}
