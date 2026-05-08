using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IDadosAlunosService
    {
        Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe(List<int> codigoAlunos, int anoLetivo, CancellationToken cancellationToken = default);
        Task<IEnumerable<AlunoRacaGeneroDto>> ObterDadosRacaGeneroAlunos(int turmaId, CancellationToken cancellationToken = default);

        Task<AlunoRacaGeneroDto?> ObterDadosRacaGeneroAlunosPorCodigoAluno(int alunoId,
            CancellationToken cancellationToken = default);
    }
}