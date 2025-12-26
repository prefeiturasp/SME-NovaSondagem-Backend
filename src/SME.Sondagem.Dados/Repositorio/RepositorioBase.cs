using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Repositorio;

[ExcludeFromCodeCoverage]
public class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    private readonly SondagemDbContext _context;

    public RepositorioBase(SondagemDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<T>> ListarAsync()
    {
        throw new NotImplementedException();
    }

    public Task<T> ObterPorIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoverAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task RemoverAsync(T entidade)
    {
        _context.Set<T>().Remove(entidade);
        await _context.SaveChangesAsync();
    }

    public Task<long> RemoverLogico(long id, string coluna = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoverLogico(long[] id, string coluna = null)
    {
        throw new NotImplementedException();
    }

    public async Task<long> SalvarAsync(T entidade)
    {
        var entryTrack = await _context.Set<T>().AddAsync(entidade);

        await _context.SaveChangesAsync();
        entryTrack.State = EntityState.Detached;
        return entryTrack.Entity.Id;
    }
}