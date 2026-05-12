using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;

public interface IExportarSondagemRelatorioPorTurmaUseCase
{
    public Task ExportarSondagemRelatorio(FiltroRelatorio filtro, CancellationToken cancellationToken);
}
