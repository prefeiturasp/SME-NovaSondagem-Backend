using MediatR;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Queries.ObterTodasTurmaElastic
{
    public class ObterTodasTurmaElasticQuery : IRequest<IEnumerable<TurmaElasticDto>>
    {
        public IEnumerable<int> TurmasId { get; set; }

        public ObterTodasTurmaElasticQuery(IEnumerable<int> turmasId)
        {
            TurmasId = turmasId;
        }
    }
}
