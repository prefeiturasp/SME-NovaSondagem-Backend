using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioOpcaoResposta
{
    Task<IEnumerable<OpcaoResposta>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<OpcaoResposta?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> CriarAsync(OpcaoResposta opcaoResposta, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(OpcaoResposta opcaoResposta, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default);
}
