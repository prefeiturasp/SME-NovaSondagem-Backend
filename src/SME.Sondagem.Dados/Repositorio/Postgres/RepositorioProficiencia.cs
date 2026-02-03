using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioProficiencia : RepositorioBase<Proficiencia>, IRepositorioProficiencia
{

    public RepositorioProficiencia(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase contextoBase) : base(context,
         servicoAuditoria, contextoBase)
    {
    }
    
    public async Task<IEnumerable<ProficienciaDto>> ObterProeficienciaPorComponenteCurricular(long componenteCurricularId,long modalidadeId,CancellationToken cancellationToken = default)
    {
        return await _context.Proficiencias
            .AsNoTracking()
            .Include(p => p.Questionarios) 
            .Where(p => p.ComponenteCurricularId == componenteCurricularId && p.ModalidadeId == modalidadeId && !p.Excluido)
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
                AlteradoRF = p.AlteradoRF,
                Questionarios = p.Questionarios.Select(q => new QuestionarioDto
                    {
                    Id = q.Id,
                    Nome = q.Nome,
                    Tipo = q.Tipo,
                    AnoLetivo = q.AnoLetivo,
                    ComponenteCurricularId = q.ComponenteCurricularId,
                    ProficienciaId = q.ProficienciaId,
                    SondagemId = q.SondagemId,
                    ModalidadeId = q.ModalidadeId,
                    SerieAno = q.SerieAno,
                    CriadoEm = q.CriadoEm,
                    CriadoPor = q.CriadoPor,
                    CriadoRF = q.CriadoRF,
                    AlteradoEm = q.AlteradoEm,
                    AlteradoPor = q.AlteradoPor,
                    AlteradoRF = q.AlteradoRF
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}