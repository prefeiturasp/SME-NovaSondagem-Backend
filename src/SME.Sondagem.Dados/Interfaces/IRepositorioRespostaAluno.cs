using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioRespostaAluno : IRepositorioBase<RespostaAluno>
{
    Task<bool> VerificarAlunoTemRespostaPorTipoQuestaoAsync(int alunoId, TipoQuestao tipoQuestao,
        CancellationToken cancellationToken);

    Task<Dictionary<int, bool>> VerificarAlunosPossuiLinguaPortuguesaAsync(List<int> alunosIds, Questao? questao, CancellationToken cancellationToken);

    Task<IEnumerable<RespostaAluno>> ObterRespostasPorSondagemEAlunosAsync(
        int sondagemId,
        IEnumerable<int> alunosIds,
        IEnumerable<int> questoesIds,
        CancellationToken cancellationToken = default);

    Task<Dictionary<(long CodigoAluno, long QuestaoId, int? BimestreId), RespostaAluno>> ObterRespostasAlunosPorQuestoesAsync(
        List<long> codigosAlunos,
        List<long> questoesIds,
        long sondagemId,
        CancellationToken cancellationToken = default);
}