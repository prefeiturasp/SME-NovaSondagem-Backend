using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Dados.Interfaces.Elastic
{
    public interface IRepositorioElasticTurma : IRepositorioElasticBase<TurmaElasticDto>
    {
        Task<TurmaElasticDto?> ObterTurmaPorId(FiltroQuestionario filtro, CancellationToken cancellationToken);
        Task<IEnumerable<TurmaElasticDto>> ObterTurmasPorIds(
            IEnumerable<int> turmaIds,
            CancellationToken cancellationToken);
        Task<IEnumerable<TurmaElasticDto>> ObterTurmasPorAnoLetivoModalidadeEAnoTurmaAsync(
            int anoLetivo, 
            int modalidade, 
            IEnumerable<string> anosTurma, 
            CancellationToken cancellationToken);
    }
}