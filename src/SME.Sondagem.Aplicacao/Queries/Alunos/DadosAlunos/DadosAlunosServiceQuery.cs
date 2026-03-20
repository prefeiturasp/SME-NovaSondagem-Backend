using MediatR;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Queries.Alunos.DadosAlunos
{
    public class DadosAlunosServiceQuery(List<string> codigoAlunos) : IRequest<IEnumerable<AlunoEolDto>>
    {
        public List<string> CodigoAlunos { get; set; } = codigoAlunos;
    }
}
