using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioRespostaAluno
{
    Task<bool> VerificarAlunoTemRespostaPorTipoQuestaoAsync(int alunoId, TipoQuestao tipoQuestao, CancellationToken cancellationToken);
    Task<Dictionary<int, bool>> VerificarAlunosTemRespostaPorTipoQuestaoAsync(List<int> alunosIds, TipoQuestao tipoQuestao, CancellationToken cancellationToken);
    Task<Dictionary<(long CodigoAluno, long QuestaoId), RespostaAluno>> ObterRespostasAlunosPorQuestoesAsync(
        List<long> codigosAlunos,
        List<long> questoesIds,
        long sondagemId,
        CancellationToken cancellationToken = default);
}