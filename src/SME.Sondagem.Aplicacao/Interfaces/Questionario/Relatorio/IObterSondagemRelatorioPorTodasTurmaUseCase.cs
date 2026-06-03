using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio
{
    public interface IObterSondagemRelatorioPorTodasTurmaUseCase
    {
        Task<FileResultDto> ObterSondagemRelatorio(string dreId, CancellationToken cancellationToken = default);
    }
}
