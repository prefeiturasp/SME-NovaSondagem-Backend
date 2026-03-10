using MediatR;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Queries.ObterTodasTurmaElastic
{
    public class ObterTodasTurmaElasticQueryHandler : IRequestHandler<ObterTodasTurmaElasticQuery, IEnumerable<TurmaElasticDto>>
    {
        protected readonly RepositoriosElastic _repositoriosElastic;

        public ObterTodasTurmaElasticQueryHandler(RepositoriosElastic repositoriosElastic)
        {
            _repositoriosElastic = repositoriosElastic ?? throw new ArgumentNullException(nameof(repositoriosElastic));
        }

        public async Task<IEnumerable<TurmaElasticDto>> Handle(ObterTodasTurmaElasticQuery request, CancellationToken cancellationToken)
           => await _repositoriosElastic.RepositorioElasticTurma.ObterTurmasPorIds(request.TurmasId,cancellationToken);
    }
}
