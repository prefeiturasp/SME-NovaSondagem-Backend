using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.Interfaces.Proficiencia;

public interface IAtualizarProficienciaUseCase
{
    Task<bool> ExecutarAsync(long id, ProficienciaDto proficienciaDto, CancellationToken cancellationToken = default);
}