using MediatR;

namespace SME.Sondagem.Aplicacao.Queries.ComponenteCurricular
{
    public class ObterComponenteCurricularPorIdQuery : IRequest<SME.Sondagem.Dominio.Entidades.ComponenteCurricular?>
    {
        public ObterComponenteCurricularPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
