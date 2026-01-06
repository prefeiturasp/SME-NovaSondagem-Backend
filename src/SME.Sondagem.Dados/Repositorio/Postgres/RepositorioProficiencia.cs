using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioProficiencia : IRepositorioProficiencia
{
    private readonly SondagemDbContext context;

    public RepositorioProficiencia(SondagemDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Proficiencia>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await context.Proficiencias
            .AsNoTracking()
            .Where(p => !p.Excluido)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Proficiencia?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Proficiencias
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);
    }

    public async Task<long> CriarAsync(Proficiencia proficiencia, CancellationToken cancellationToken = default)
    {
        await context.Proficiencias.AddAsync(proficiencia, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return proficiencia.Id;
    }

    public async Task<bool> AtualizarAsync(Proficiencia proficiencia, CancellationToken cancellationToken = default)
    {
        var proficienciaExistente = await context.Proficiencias
            .FirstOrDefaultAsync(p => p.Id == proficiencia.Id && !p.Excluido, cancellationToken);

        if (proficienciaExistente == null)
            return false;

        proficienciaExistente.AlteradoEm = proficiencia.AlteradoEm;
        proficienciaExistente.AlteradoPor = proficiencia.AlteradoPor;
        proficienciaExistente.AlteradoRF = proficiencia.AlteradoRF;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var proficiencia = await context.Proficiencias
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);

        if (proficiencia == null)
            return false;

        proficiencia.Excluido = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}