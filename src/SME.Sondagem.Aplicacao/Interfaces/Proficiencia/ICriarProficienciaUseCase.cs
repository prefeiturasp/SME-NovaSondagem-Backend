using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.Interfaces.Proficiencia;

public interface ICriarProficienciaUseCase
{
    Task<long> ExecutarAsync(ProficienciaDto proficienciaDto, CancellationToken cancellationToken = default);
}