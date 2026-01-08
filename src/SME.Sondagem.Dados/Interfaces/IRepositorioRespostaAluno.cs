using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioRespostaAluno
{
    Task<bool> VerificarAlunoTemRespostaPorTipoQuestaoAsync(int alunoId, TipoQuestao tipoQuestao, CancellationToken cancellationToken);
    Task<Dictionary<int, bool>> VerificarAlunosTemRespostaPorTipoQuestaoAsync(List<int> alunosIds, TipoQuestao tipoQuestao, CancellationToken cancellationToken);
    Task<Dictionary<(int AlunoId, int CicloId), (int Id, int OpcaoRespostaId)>> ObterRespostasAlunosPorCiclosAsync(List<int> alunosIds, List<int> ciclosIds, int proficienciaId, int ano, CancellationToken cancellationToken);
}