using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Repositorio;

[ExcludeFromCodeCoverage]
public class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    protected readonly SondagemDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public RepositorioBase(SondagemDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> ListarAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<T?> ObterPorIdAsync(long id)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<long> SalvarAsync(T entidade)
    {
        if (entidade.Id == 0)
        {
            await _dbSet.AddAsync(entidade);
        }
        else
        {
            _dbSet.Update(entidade);
        }

        await _context.SaveChangesAsync();
        return entidade.Id;
    }

    public virtual async Task RemoverAsync(long id)
    {
        var entidade = await _dbSet.FindAsync((int)id);
        if (entidade != null)
        {
            _dbSet.Remove(entidade);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task RemoverAsync(T entidade)
    {
        _dbSet.Remove(entidade);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<long> RemoverLogico(long id, string coluna = null)
    {
        // Adicione IgnoreQueryFilters para deixar explícito
        var entidade = await _dbSet
            .IgnoreQueryFilters() // Importante!
            .FirstOrDefaultAsync(e => e.Id == (int)id);

        if (entidade == null)
            return 0;

        entidade.Excluido = true;
        await _context.SaveChangesAsync();

        return entidade.Id;
    }

    public virtual async Task<bool> RemoverLogico(long[] ids, string coluna = null)
    {
        if (ids == null || ids.Length == 0)
            return false;

        var entidades = await _dbSet
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();

        if (!entidades.Any())
            return false;

        foreach (var entidade in entidades)
        {
            entidade.Excluido = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> RestaurarAsync(long id)
    {
        var entidade = await _dbSet
            .IgnoreQueryFilters() // Importante!
            .FirstOrDefaultAsync(e => e.Id == (int)id);

        if (entidade == null)
            return false;

        entidade.Excluido = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public virtual async Task<IEnumerable<T>> ListarTodosIncluindoExcluidosAsync()
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToListAsync();
    }
}