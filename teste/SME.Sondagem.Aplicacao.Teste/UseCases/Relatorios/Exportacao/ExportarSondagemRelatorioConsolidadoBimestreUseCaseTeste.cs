using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;

namespace SME.Sondagem.Testes.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioConsolidadoBimestreUseCaseTeste
    : ExportarSondagemRelatorioConsolidadoUseCaseBaseTeste<ExportarSondagemRelatorioConsolidadoBimestreUseCase>
{
    protected override TipoRelatorio TipoRelatorioEsperado => TipoRelatorio.ConsolidadoPorBimestre;

    protected override string RotaRabbitEsperada => RotasRabbit.RelatorioSondagemConsolidadoPorBimestre;

    protected override ExportarSondagemRelatorioConsolidadoBimestreUseCase CriarUseCase()
        => new(
            MockSolicitacaoRelatorioService.Object,
            MockServicoLog.Object,
            MockServicoMensageria.Object,
            MockServicoUsuario.Object);
}
