using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioBimestre : IRepositorioBimestre
{
    private readonly SondagemDbContext context;

    public RepositorioBimestre(SondagemDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Bimestre>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await context.Bimestres
            .AsNoTracking()
            .Where(p => !p.Excluido)
            .OrderBy(p => p.Descricao)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bimestre?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Bimestres
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);
    }

    public async Task<long> CriarAsync(Bimestre bimestre, CancellationToken cancellationToken = default)
    {
        await context.Bimestres.AddAsync(bimestre, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return bimestre.Id;
    }

    public async Task<bool> AtualizarAsync(Bimestre bimestre, CancellationToken cancellationToken = default)
    {
        var bimestreExistente = await context.Bimestres
            .FirstOrDefaultAsync(p => p.Id == bimestre.Id && !p.Excluido, cancellationToken);

        if (bimestreExistente == null)
            return false;

        bimestreExistente.AlteradoEm = bimestre.AlteradoEm;
        bimestreExistente.AlteradoPor = bimestre.AlteradoPor;
        bimestreExistente.AlteradoRF = bimestre.AlteradoRF;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var bimestre = await context.Bimestres
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);

        if (bimestre == null)
            return false;

        bimestre.Excluido = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}