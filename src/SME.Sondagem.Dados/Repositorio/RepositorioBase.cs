using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Repositorio;

public class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    protected readonly SondagemDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public RepositorioBase(SondagemDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<long> SalvarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        if (entidade.Id == 0)
        {
            await _dbSet.AddAsync(entidade, cancellationToken);
        }
        else
        {
            var entidadeExistente = await _dbSet.FindAsync(new object[] { entidade.Id }, cancellationToken);

            if (entidadeExistente != null)
            {
                _context.Entry(entidadeExistente).CurrentValues.SetValues(entidade);
            }
            else
            {
                _dbSet.Update(entidade);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return entidade.Id;
    }

    public virtual async Task RemoverAsync(long id, CancellationToken cancellationToken = default)
    {
        var entidade = await _dbSet.FindAsync(new object[] { (int)id }, cancellationToken);
        if (entidade != null)
        {
            _dbSet.Remove(entidade);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task RemoverAsync(T entidade, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entidade);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<long> RemoverLogico(long id, string? coluna = null, CancellationToken cancellationToken = default)
    {
        var entidade = await _dbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == (int)id, cancellationToken);

        if (entidade == null)
            return 0;

        entidade.Excluido = true;
        await _context.SaveChangesAsync(cancellationToken);

        return entidade.Id;
    }

    public virtual async Task<bool> RemoverLogico(long[] ids, string? coluna = null, CancellationToken cancellationToken = default)
    {
        if (ids == null || ids.Length == 0)
            return false;

        var entidades = await _dbSet
            .IgnoreQueryFilters()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (entidades.Count == 0)
            return false;

        foreach (var entidade in entidades)
        {
            entidade.Excluido = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public virtual async Task<bool> RestaurarAsync(long id, CancellationToken cancellationToken = default)
    {
        var entidade = await _dbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == (int)id, cancellationToken);

        if (entidade == null)
            return false;

        entidade.Excluido = false;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public virtual async Task<IEnumerable<T>> ListarTodosIncluindoExcluidosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}