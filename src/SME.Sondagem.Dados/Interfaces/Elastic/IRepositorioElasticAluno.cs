using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Dados.Interfaces.Elastic
{
    public interface IRepositorioElasticAluno : IRepositorioElasticBase<AlunoElasticDto>
    {
        Task<IEnumerable<AlunoElasticDto>> ObterAlunosPorIdTurma(int idTurma, CancellationToken cancellationToken);
    }
}