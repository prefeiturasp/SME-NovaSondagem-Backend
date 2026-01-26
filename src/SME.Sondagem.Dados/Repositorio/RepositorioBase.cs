using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Contexto;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Repositorio;

[ExcludeFromCodeCoverage]
public class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    protected readonly SondagemDbContext _context;
    private readonly IServicoAuditoria _servicoAuditoria;
    private readonly ContextoBase _database;
    protected readonly DbSet<T> _dbSet;

    public RepositorioBase(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase database)
    {
        _context = context;
        _servicoAuditoria = servicoAuditoria;
        _dbSet = context.Set<T>();
        _database = database;
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
        var dataAtual = GerarDataHoraAtual();
        if (entidade.Id == 0)
        {
            CriarDadosUsuarioCriacao(entidade);
            await _dbSet.AddAsync(entidade, cancellationToken);
            await _servicoAuditoria.AuditarAsync(typeof(T).Name.ToLower(), entidade.Id, "I").WaitAsync(cancellationToken);
        }
        else
        {
            var entidadeExistente = await _dbSet.FindAsync([entidade.Id], cancellationToken);
            CriarDadosUsuarioAlteracao(entidade,dataAtual);
            if (entidadeExistente != null)
            {
                _context.Entry(entidadeExistente).CurrentValues.SetValues(entidade);
            }
            else
            {
                _dbSet.Update(entidade);
            }

            await _servicoAuditoria.AuditarAsync(typeof(T).Name.ToLower(), entidade.Id, "A")
                .WaitAsync(cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return entidade.Id;
    }

    public virtual async Task<bool> SalvarAsync(List<T> entidades, CancellationToken cancellationToken = default)
    {
        if (entidades.Count == 0)
            return false;

        var idsAlteracao = new List<long>();
        var dataAtual = GerarDataHoraAtual();
        foreach (var entidade in entidades)
        {
            if (entidade.Id == 0)
            {
                CriarDadosUsuarioCriacao(entidade);
                await _dbSet.AddAsync(entidade, cancellationToken);
            }
            else
            {
                var entidadeExistente = await _dbSet.FindAsync([entidade.Id], cancellationToken);
                CriarDadosUsuarioAlteracao(entidade,dataAtual);
                if (entidadeExistente != null)
                {
                    _context.Entry(entidadeExistente).CurrentValues.SetValues(entidade);
                }
                else
                {
                    _dbSet.Update(entidade);
                }

                idsAlteracao.Add(entidade.Id);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        var idsInsercao = entidades.Where(e => !idsAlteracao.Contains(e.Id)).Select(e => (long)e.Id).ToList();
        if (idsInsercao.Count > 0)
        {
            await _servicoAuditoria.AuditarMultiplosAsync(typeof(T).Name.ToLower(), idsInsercao, "I")
                .WaitAsync(cancellationToken);
        }

        if (idsAlteracao.Count > 0)
        {
            await _servicoAuditoria.AuditarMultiplosAsync(typeof(T).Name.ToLower(), idsAlteracao, "A")
                .WaitAsync(cancellationToken);
        }

        return true;
    }

    public virtual async Task RemoverAsync(long id, CancellationToken cancellationToken = default)
    {
        var entidade = await _dbSet.FindAsync(new object[] { (int)id }, cancellationToken);
        if (entidade != null)
        {
            _dbSet.Remove(entidade);
            await _context.SaveChangesAsync(cancellationToken);
            await _servicoAuditoria.AuditarAsync(typeof(T).Name.ToLower(), entidade.Id, "E")
                .WaitAsync(cancellationToken);
        }
    }

    public virtual async Task RemoverAsync(T entidade, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entidade);
        await _context.SaveChangesAsync(cancellationToken);
        await _servicoAuditoria.AuditarAsync(typeof(T).Name.ToLower(), entidade.Id, "E").WaitAsync(cancellationToken);
    }

    public virtual async Task<long> RemoverLogico(long id, string? coluna = null,
        CancellationToken cancellationToken = default)
    {
        var entidade = await _dbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == (int)id, cancellationToken);

        if (entidade == null)
            return 0;

        entidade.Excluido = true;
        await _context.SaveChangesAsync(cancellationToken);
        await _servicoAuditoria.AuditarAsync(typeof(T).Name.ToLower(), entidade.Id, "E").WaitAsync(cancellationToken);
        return entidade.Id;
    }

    public virtual async Task<bool> RemoverLogico(long[] ids, string? coluna = null,
        CancellationToken cancellationToken = default)
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

        await _servicoAuditoria.AuditarMultiplosAsync(typeof(T).Name.ToLower(), ids, "A").WaitAsync(cancellationToken);
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
        await _servicoAuditoria.AuditarAsync(typeof(T).Name.ToLower(), entidade.Id, "R").WaitAsync(cancellationToken);
        return true;
    }

    public virtual async Task<IEnumerable<T>> ListarTodosIncluindoExcluidosAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private static DateTime GerarDataHoraAtual()
    {
        var agora = DateTime.Now;
        var atual = new DateTime(
            agora.Year, agora.Month, agora.Day,
            agora.Hour, agora.Minute, agora.Second,
            DateTimeKind.Unspecified);
        return atual.AddHours(-6);
    }
    private void CriarDadosUsuarioCriacao(T entidade)
    {
        entidade.CriadoEm = DateTime.UtcNow;
        entidade.CriadoPor = _database.UsuarioLogado;
        entidade.CriadoRF = _database.UsuarioLogadoRf;
    }
    private void CriarDadosUsuarioAlteracao(T entidade,DateTime data)
    {
        entidade.AlteradoEm = data;
        entidade.AlteradoPor = _database.UsuarioLogado;
        entidade.AlteradoRF = _database.UsuarioLogadoRf;
    }
}