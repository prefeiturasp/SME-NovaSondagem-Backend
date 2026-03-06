using MediatR;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Queries.Alunos.DadosAlunos
{
    public class DadosAlunosServiceQuery(string codigoUe) : IRequest<IEnumerable<AlunoEolDto>>
    {
        public string CodigoUe  { get; set; } = codigoUe;
    }
}