using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;

namespace SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;

public interface IComponenteCurricularUseCase
{
    Task<ComponenteCurricularDto> CriarAsync(CriarComponenteCurricularDto dto, CancellationToken cancellationToken = default);
    Task<ComponenteCurricularDto> AtualizarAsync(int id, AtualizarComponenteCurricularDto dto, CancellationToken cancellationToken = default);
    Task<ComponenteCurricularDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ComponenteCurricularDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default);
    Task<ComponenteCurricularDto?> ObterPorCodigoEolAsync(int codigoEol, CancellationToken cancellationToken = default);
}