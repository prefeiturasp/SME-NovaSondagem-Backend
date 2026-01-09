using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioProficiencia
{
    Task<IEnumerable<Proficiencia>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProficienciaDto>> ObterProeficienciaPorComponenteCurricular(long componenteCurricularId,CancellationToken cancellationToken = default);
    Task<Proficiencia?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> CriarAsync(Proficiencia proficiencia, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(Proficiencia proficiencia, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default);
}
