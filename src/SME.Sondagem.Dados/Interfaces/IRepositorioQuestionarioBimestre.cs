using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioQuestionarioBimestre
{
    Task<IEnumerable<QuestionarioBimestre>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<QuestionarioBimestre>> ObterPorQuestionarioIdAsync(int questionarioId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuestionarioBimestre>> ObterPorBimestreIdAsync(int bimestreId, CancellationToken cancellationToken = default);
    Task<bool> ExisteVinculoAsync(int questionarioId, int bimestreId, CancellationToken cancellationToken = default);
    Task<bool> CriarMultiplosAsync(List<QuestionarioBimestre> questionariosBimestres, CancellationToken cancellationToken = default);
    Task<bool> ExcluirPorQuestionarioIdAsync(int questionarioId, CancellationToken cancellationToken = default);
    Task<bool> ExcluirPorQuestionarioEBimestreAsync(int questionarioId, int bimestreId, CancellationToken cancellationToken = default);
}