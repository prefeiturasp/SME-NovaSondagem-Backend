using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;

namespace SME.Sondagem.Testes.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioConsolidadoAnoUseCaseTeste
    : ExportarSondagemRelatorioConsolidadoUseCaseBaseTeste<ExportarSondagemRelatorioConsolidadoAnoUseCase>
{
    protected override TipoRelatorio TipoRelatorioEsperado => TipoRelatorio.ConsolidadoPorAno;

    protected override string RotaRabbitEsperada => RotasRabbit.RelatorioSondagemConsolidadoPorAno;

    protected override ExportarSondagemRelatorioConsolidadoAnoUseCase CriarUseCase()
        => new(
            MockSolicitacaoRelatorioService.Object,
            MockServicoLog.Object,
            MockServicoMensageria.Object,
            MockServicoUsuario.Object);
}
