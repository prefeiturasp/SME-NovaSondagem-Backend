using MediatR;

namespace SME.Sondagem.Aplicacao.Queries.Bimestre
{
    public class ObterBimestrePorIdQuery : IRequest<SME.Sondagem.Dominio.Entidades.Bimestre?>
    {
        public ObterBimestrePorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
