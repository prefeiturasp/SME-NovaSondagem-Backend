using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.Interfaces.Proficiencia;

public interface IObterProficienciasPorComponenteCurricularUseCase
{
    Task<IEnumerable<ProficienciaDto>> ExecutarAsync(long componenteCurricularId,CancellationToken cancellationToken = default);
}