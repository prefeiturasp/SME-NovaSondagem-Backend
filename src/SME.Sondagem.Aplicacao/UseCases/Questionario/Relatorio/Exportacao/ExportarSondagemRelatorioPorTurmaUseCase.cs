using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioPorTurmaUseCase
    : ExportarSondagemRelatorioConsolidadoUseCaseBase<FiltroRelatorio>,
      IExportarSondagemRelatorioPorTurmaUseCase
{
    public ExportarSondagemRelatorioPorTurmaUseCase(
        ISolicitacaoRelatorioService solicitacaoRelatorioService,
        IServicoLog servicoLog,
        IServicoMensageria servicoMensageria,
        IServicoUsuario servicoUsuario)
        : base(solicitacaoRelatorioService, servicoLog, servicoMensageria, servicoUsuario)
    {
    }

    protected override TipoRelatorio TipoRelatorio => TipoRelatorio.SondagemPorTurma;

    protected override string RotaRabbit => RotasRabbit.RelatorioSondagemPorTurma;

    public Task ExportarSondagemRelatorio(FiltroRelatorio filtro, CancellationToken cancellationToken)
        => Exportar(filtro, cancellationToken);
}
