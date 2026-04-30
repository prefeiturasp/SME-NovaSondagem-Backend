using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;

namespace SME.Sondagem.Testes.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioConsolidadoRacaGeneroUseCaseTeste
    : ExportarSondagemRelatorioConsolidadoUseCaseBaseTeste<ExportarSondagemRelatorioConsolidadoRacaGeneroUseCase>
{
    protected override TipoRelatorio TipoRelatorioEsperado => TipoRelatorio.ConsolidadoPorRacaGenero;

    protected override string RotaRabbitEsperada => RotasRabbit.RelatorioSondagemConsolidadoPorRacaGenero;

    protected override ExportarSondagemRelatorioConsolidadoRacaGeneroUseCase CriarUseCase()
        => new(
            MockSolicitacaoRelatorioService.Object,
            MockServicoLog.Object,
            MockServicoMensageria.Object,
            MockServicoUsuario.Object);
}
