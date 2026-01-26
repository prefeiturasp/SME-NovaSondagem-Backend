using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioBimestre
{
    Task<IEnumerable<Bimestre>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<Bimestre?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> CriarAsync(Bimestre bimestre, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(Bimestre bimestre, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<SondagemPeriodoBimestre>> ObterBimestresPorQuestionarioIdAsync(int questionarioId, CancellationToken cancellationToken = default);
}