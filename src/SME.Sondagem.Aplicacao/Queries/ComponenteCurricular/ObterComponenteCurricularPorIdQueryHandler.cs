using MediatR;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.Queries.ComponenteCurricular
{
    public class ObterComponenteCurricularPorIdQueryHandler : IRequestHandler<ObterComponenteCurricularPorIdQuery, SME.Sondagem.Dominio.Entidades.ComponenteCurricular?>
    {
        private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;

        public ObterComponenteCurricularPorIdQueryHandler(IRepositorioComponenteCurricular repositorioComponenteCurricular)
        {
            _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
        }

        public async Task<Dominio.Entidades.ComponenteCurricular?> Handle(ObterComponenteCurricularPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioComponenteCurricular.ObterPorIdAsync(request.Id, cancellationToken);
        }
    }
}
