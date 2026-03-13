using MediatR;
using SME.Sondagem.Dados.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Aplicacao.Queries.Bimestre
{
    public class ObterBimestrePorIdQueryHandler : IRequestHandler<ObterBimestrePorIdQuery, SME.Sondagem.Dominio.Entidades.Bimestre?>
    {
        private readonly IRepositorioBimestre _repositorioBimestre;

        public ObterBimestrePorIdQueryHandler(IRepositorioBimestre repositorioBimestre)
        {
            _repositorioBimestre = repositorioBimestre ?? throw new ArgumentNullException(nameof(repositorioBimestre));
        }

        public async Task<Dominio.Entidades.Bimestre?> Handle(ObterBimestrePorIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioBimestre.ObterPorIdAsync(request.Id, cancellationToken);
        }
    }
}
