using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Dados.Interfaces.Elastic
{
    public interface IRepositorioElasticTurma : IRepositorioElasticBase<TurmaElasticDto>
    {
        Task<TurmaElasticDto> ObterTurmaPorId(FiltroQuestionario filtro, CancellationToken cancellationToken);
    }
}