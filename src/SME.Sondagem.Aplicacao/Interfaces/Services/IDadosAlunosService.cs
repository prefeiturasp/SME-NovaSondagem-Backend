using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IDadosAlunosService
    {
        Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe(List<int> codigoAlunos, int anoLetivo, CancellationToken cancellationToken = default);
        Task<IEnumerable<AlunoRacaGeneroDto>> ObterDadosRacaGeneroAlunos(int turmaId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AlunoRacaGeneroDto>> ObterDadosRacaGeneroAlunosForaDaListaTurma(IEnumerable<int> codigosAlunos, IEnumerable<RespostaAluno> respostasSalvas, CancellationToken cancellationToken = default);
    }
}