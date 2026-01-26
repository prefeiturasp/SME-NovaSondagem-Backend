using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Sondagem;

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

    public async Task<ICollection<SondagemPeriodoBimestre>> ObterBimestresPorQuestionarioIdAsync(int questionarioId, CancellationToken cancellationToken = default)
    {
        // Consulta a tabela questionario_bimestre para obter os bimestreIds associados ao questionario
        var bimestreIds = await context.QuestionariosBimestres
            .AsNoTracking()
            .Where(qb => qb.QuestionarioId == questionarioId && !qb.Excluido)
            .Select(qb => qb.BimestreId)
            .ToListAsync(cancellationToken);

        if (!bimestreIds.Any())
            return new List<SondagemPeriodoBimestre>();

        var questionario = await context.Questionarios
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == questionarioId, cancellationToken);

        if (questionario == null)
            return new List<SondagemPeriodoBimestre>();

        var sondagemId = questionario.SondagemId;

        var periodos = await context.SondagemPeriodosBimestre
            .Include(sp => sp.Bimestre)
            .AsNoTracking()
            .Where(sp => sp.SondagemId == sondagemId
                         && bimestreIds.Contains(sp.BimestreId)
                         && !sp.Excluido)
            .ToListAsync(cancellationToken);

        return periodos;
    }
}