using SME.Sondagem.Dados.Repositorio;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioProficiencia : IRepositorioBase<Proficiencia>
{
    Task<IEnumerable<ProficienciaDto>> ObterProeficienciaPorComponenteCurricular(long componenteCurricularId,long modalidadeId,CancellationToken cancellationToken = default);
}
