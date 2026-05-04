using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IDadosAlunosService
    {
        Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe(List<string> codigoAlunos, CancellationToken cancellationToken = default);
        Task<IEnumerable<AlunoRacaGeneroDto>> ObterDadosRacaGeneroAlunos(int turmaId, CancellationToken cancellationToken = default);
    }
}