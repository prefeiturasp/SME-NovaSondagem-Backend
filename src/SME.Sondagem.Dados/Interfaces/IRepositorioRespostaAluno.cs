using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dados.Interfaces
{
    public interface IRepositorioRespostaAluno : IRepositorioBase<RespostaAluno>
    {
        Task<IEnumerable<RespostaAluno>> ObterRespostasPorSondagemEAlunosAsync(
            int sondagemId,
            IEnumerable<int> alunosIds,
            IEnumerable<int> questoesIds,
            CancellationToken cancellationToken = default);
    }
}
