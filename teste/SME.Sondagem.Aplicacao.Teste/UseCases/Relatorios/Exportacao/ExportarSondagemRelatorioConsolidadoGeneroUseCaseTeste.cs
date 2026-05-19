using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;

namespace SME.Sondagem.Testes.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioConsolidadoGeneroUseCaseTeste
    : ExportarSondagemRelatorioConsolidadoUseCaseBaseTeste<ExportarSondagemRelatorioConsolidadoGeneroUseCase>
{
    protected override TipoRelatorio TipoRelatorioEsperado => TipoRelatorio.ConsolidadoPorGenero;

    protected override string RotaRabbitEsperada => RotasRabbit.RelatorioSondagemConsolidadoPorGenero;

    protected override ExportarSondagemRelatorioConsolidadoGeneroUseCase CriarUseCase()
        => new(
            MockSolicitacaoRelatorioService.Object,
            MockServicoLog.Object,
            MockServicoMensageria.Object,
            MockServicoUsuario.Object);
}
