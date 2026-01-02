using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioQuestao
{
    Task<IEnumerable<Questao>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<Questao?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> CriarAsync(Questao questao, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(Questao questao, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default);
}
