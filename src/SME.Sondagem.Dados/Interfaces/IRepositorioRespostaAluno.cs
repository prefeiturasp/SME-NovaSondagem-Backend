using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infra.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Dados.Interfaces
{
    public interface IRepositorioRespostaAluno : IRepositorioBase<RespostaAluno>
    {
        Task<IEnumerable<long>> SalvarSondagemAsync(int sondagemId, List<AlunoSondagemDto> alunos);
    }


}
