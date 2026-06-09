using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio
{
    public interface IObterSondagemRelatorioPorTodasTurmaUseCase
    {
        Task<FileResultDto?> ObterSondagemRelatorio(FiltroExtracaoDadosDTO filtroExtracaoDados, CancellationToken cancellationToken = default);
    }
}
