using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioCiclo : IRepositorioCiclo
{
    private readonly SondagemDbContext context;

    public RepositorioCiclo(SondagemDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Ciclo>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await context.Ciclos
            .AsNoTracking()
            .Where(p => !p.Excluido)
            .OrderBy(p => p.DescCiclo)
            .ToListAsync(cancellationToken);
    }

    public async Task<Ciclo?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Ciclos
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);
    }

    public async Task<long> CriarAsync(Ciclo ciclo, CancellationToken cancellationToken = default)
    {
        await context.Ciclos.AddAsync(ciclo, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return ciclo.Id;
    }

    public async Task<bool> AtualizarAsync(Ciclo ciclo, CancellationToken cancellationToken = default)
    {
        var cicloExistente = await context.Ciclos
            .FirstOrDefaultAsync(p => p.Id == ciclo.Id && !p.Excluido, cancellationToken);

        if (cicloExistente == null)
            return false;

        cicloExistente.AlteradoEm = ciclo.AlteradoEm;
        cicloExistente.AlteradoPor = ciclo.AlteradoPor;
        cicloExistente.AlteradoRF = ciclo.AlteradoRF;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var ciclo = await context.Ciclos
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);

        if (ciclo == null)
            return false;

        ciclo.Excluido = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
