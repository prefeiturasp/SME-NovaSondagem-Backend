using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioQuestaoOpcaoResposta
{
    Task<IEnumerable<QuestaoOpcaoResposta>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<QuestaoOpcaoResposta?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> CriarAsync(QuestaoOpcaoResposta questaoOpcaoResposta, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(QuestaoOpcaoResposta questaoOpcaoResposta, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default);
}
