using MediatR;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.Queries.Proficiencia
{
    public class ObterProficienciaPorIdQueryHandler : IRequestHandler<ObterProficienciaPorIdQuery, SME.Sondagem.Dominio.Entidades.Proficiencia?>
    {
        private readonly IRepositorioProficiencia _proficienciaRepositorio;

        public ObterProficienciaPorIdQueryHandler(IRepositorioProficiencia proficienciaRepositorio)
        {
            _proficienciaRepositorio = proficienciaRepositorio ?? throw new ArgumentNullException(nameof(proficienciaRepositorio));
        }

        public async Task<Dominio.Entidades.Proficiencia?> Handle(ObterProficienciaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _proficienciaRepositorio.ObterPorIdAsync(request.Id, cancellationToken);
        }
    }
}
