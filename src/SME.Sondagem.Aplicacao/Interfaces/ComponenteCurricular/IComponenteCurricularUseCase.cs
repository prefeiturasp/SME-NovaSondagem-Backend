using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;

namespace SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;

public interface IComponenteCurricularUseCase
{
    Task<ComponenteCurricularDto> CriarAsync(CriarComponenteCurricularDto dto);
    Task<ComponenteCurricularDto> AtualizarAsync(int id, AtualizarComponenteCurricularDto dto);
    Task<ComponenteCurricularDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<ComponenteCurricularDto>> ListarAsync();
    Task<bool> ExcluirAsync(int id);
    Task<ComponenteCurricularDto?> ObterPorCodigoEolAsync(int codigoEol);
}