using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;

namespace SME.Sondagem.Testes.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioConsolidadoRacaUseCaseTeste
    : ExportarSondagemRelatorioConsolidadoUseCaseBaseTeste<ExportarSondagemRelatorioConsolidadoRacaUseCase>
{
    protected override TipoRelatorio TipoRelatorioEsperado => TipoRelatorio.ConsolidadoPorRaca;

    protected override string RotaRabbitEsperada => RotasRabbit.RelatorioSondagemConsolidadoPorRaca;

    protected override ExportarSondagemRelatorioConsolidadoRacaUseCase CriarUseCase()
        => new(
            MockSolicitacaoRelatorioService.Object,
            MockServicoLog.Object,
            MockServicoMensageria.Object,
            MockServicoUsuario.Object);
}
