using MediatR;

namespace SME.Sondagem.Aplicacao.Queries.Proficiencia
{
    public class ObterProficienciaPorIdQuery : IRequest<SME.Sondagem.Dominio.Entidades.Proficiencia?>
    {
        public ObterProficienciaPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
