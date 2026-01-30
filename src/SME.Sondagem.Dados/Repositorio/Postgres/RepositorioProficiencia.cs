using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioProficiencia : RepositorioBase<Proficiencia>, IRepositorioProficiencia
{

    public RepositorioProficiencia(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase contextoBase) : base(context,
         servicoAuditoria, contextoBase)
    {
    }
    
    public async Task<IEnumerable<ProficienciaDto>> ObterProeficienciaPorComponenteCurricular(long componenteCurricularId,CancellationToken cancellationToken = default)
    {
        return await _context.Proficiencias
            .AsNoTracking()
            .Where(p => p.ComponenteCurricularId == componenteCurricularId && !p.Excluido)
            .OrderBy(p => p.Nome)
            .Select(p => new ProficienciaDto 
            { 
                Id = p.Id,
                Nome = p.Nome,
                ComponenteCurricularId = p.ComponenteCurricularId,
                CriadoEm = p.CriadoEm,
                CriadoPor = p.CriadoPor,
                CriadoRF = p.CriadoRF,
                AlteradoEm = p.AlteradoEm,
                AlteradoPor = p.AlteradoPor,
                AlteradoRF = p.AlteradoRF
            })
            .ToListAsync(cancellationToken);
    }
}