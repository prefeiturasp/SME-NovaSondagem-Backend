using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Services.Auditoria;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioComponenteCurricular : RepositorioBase<ComponenteCurricular>, IRepositorioComponenteCurricular
{
    public RepositorioComponenteCurricular(SondagemDbContext context, IServicoAuditoria servicoAuditoria) : base(
        context,
        servicoAuditoria)
    {
    }

    public async Task<ComponenteCurricular?> ObterPorCodigoEolAsync(int codigoEol,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CodigoEol == codigoEol, cancellationToken);
    }

    public async Task<IEnumerable<ComponenteCurricular>> ObterPorAnoAsync(int ano,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.Ano == ano)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteComCodigoEolAsync(int codigoEol, int? idIgnorar = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (idIgnorar.HasValue)
            query = query.Where(c => c.Id != idIgnorar.Value);

        return await query.AnyAsync(c => c.CodigoEol == codigoEol, cancellationToken);
    }
}