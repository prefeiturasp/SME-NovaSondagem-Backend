using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.Interfaces.Proficiencia;

public interface IObterProficienciasUseCase
{
    Task<IEnumerable<ProficienciaDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}