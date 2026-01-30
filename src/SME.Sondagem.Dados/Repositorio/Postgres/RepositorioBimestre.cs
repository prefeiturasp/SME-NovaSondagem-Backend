using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioBimestre : RepositorioBase<Bimestre>, IRepositorioBimestre
{

    public RepositorioBimestre(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase contextoBase) : base(context,
    servicoAuditoria, contextoBase)
    {
    }

    public async Task<ICollection<SondagemPeriodoBimestre>> ObterBimestresPorQuestionarioIdAsync(int questionarioId, CancellationToken cancellationToken = default)
    {
        var bimestreIds = await _context.QuestionariosBimestres
            .AsNoTracking()
            .Where(qb => qb.QuestionarioId == questionarioId && !qb.Excluido)
            .Select(qb => qb.BimestreId)
            .ToListAsync(cancellationToken);

        if (bimestreIds.Count == 0)
            return new List<SondagemPeriodoBimestre>();

        var questionario = await _context.Questionarios
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == questionarioId, cancellationToken);

        if (questionario == null)
            return new List<SondagemPeriodoBimestre>();

        var sondagemId = questionario.SondagemId;

        var periodos = await _context.SondagemPeriodosBimestre
            .Include(sp => sp.Bimestre)
            .AsNoTracking()
            .Where(sp => sp.SondagemId == sondagemId
                         && bimestreIds.Contains(sp.BimestreId)
                         && !sp.Excluido)
            .ToListAsync(cancellationToken);

        return periodos;
    }
}