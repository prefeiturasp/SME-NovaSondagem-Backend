using MediatR;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Queries.Alunos.DadosAlunos
{
    public class DadosAlunosServiceQueryHandler : IRequestHandler<DadosAlunosServiceQuery,IEnumerable<AlunoEolDto>>
    {
        private readonly IDadosAlunosService _alunosService;

        public DadosAlunosServiceQueryHandler(IDadosAlunosService alunosService)
        {
            _alunosService = alunosService ?? throw new ArgumentNullException(nameof(alunosService));
        }

        public Task<IEnumerable<AlunoEolDto>> Handle(DadosAlunosServiceQuery request, CancellationToken cancellationToken)
                => _alunosService.ObterDadosAlunosPorCodigoUe(request.CodigoUe, cancellationToken);
        
    }
}