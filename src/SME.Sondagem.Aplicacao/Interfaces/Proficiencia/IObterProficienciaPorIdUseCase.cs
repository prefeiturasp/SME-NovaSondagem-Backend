using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.Interfaces.Proficiencia;

public interface IObterProficienciaPorIdUseCase
{
    Task<ProficienciaDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}