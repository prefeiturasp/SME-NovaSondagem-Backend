using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioQuestionarioBimestre : IRepositorioQuestionarioBimestre
{
    private readonly SondagemDbContext _context;

    public RepositorioQuestionarioBimestre(SondagemDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<QuestionarioBimestre>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.QuestionariosBimestres
            .Include(qb => qb.Questionario)
            .Include(qb => qb.Bimestre)
            .AsNoTracking()
            .Where(qb => !qb.Excluido)
            .OrderBy(qb => qb.QuestionarioId)
            .ThenBy(qb => qb.BimestreId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuestionarioBimestre>> ObterPorQuestionarioIdAsync(int questionarioId, CancellationToken cancellationToken = default)
    {
        return await _context.QuestionariosBimestres
            .Include(qb => qb.Bimestre)
            .AsNoTracking()
            .Where(qb => qb.QuestionarioId == questionarioId && !qb.Excluido)
            .OrderBy(qb => qb.BimestreId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuestionarioBimestre>> ObterPorBimestreIdAsync(int bimestreId, CancellationToken cancellationToken = default)
    {
        return await _context.QuestionariosBimestres
            .Include(qb => qb.Questionario)
            .AsNoTracking()
            .Where(qb => qb.BimestreId == bimestreId && !qb.Excluido)
            .OrderBy(qb => qb.QuestionarioId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteVinculoAsync(int questionarioId, int bimestreId, CancellationToken cancellationToken = default)
    {
        return await _context.QuestionariosBimestres
            .AsNoTracking()
            .AnyAsync(qb => qb.QuestionarioId == questionarioId
                         && qb.BimestreId == bimestreId
                         && !qb.Excluido, cancellationToken);
    }

    public async Task<bool> CriarMultiplosAsync(List<QuestionarioBimestre> questionariosBimestres, CancellationToken cancellationToken = default)
    {
        if (questionariosBimestres == null || questionariosBimestres.Count == 0)
            return false;

        await _context.QuestionariosBimestres.AddRangeAsync(questionariosBimestres, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirPorQuestionarioIdAsync(int questionarioId, CancellationToken cancellationToken = default)
    {
        var vinculos = await _context.QuestionariosBimestres
            .Where(qb => qb.QuestionarioId == questionarioId && !qb.Excluido)
            .ToListAsync(cancellationToken);

        if (vinculos.Count == 0)
            return false;

        foreach (var vinculo in vinculos)
        {
            vinculo.Excluido = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirPorQuestionarioEBimestreAsync(int questionarioId, int bimestreId, CancellationToken cancellationToken = default)
    {
        var vinculo = await _context.QuestionariosBimestres
            .FirstOrDefaultAsync(qb => qb.QuestionarioId == questionarioId
                                    && qb.BimestreId == bimestreId
                                    && !qb.Excluido, cancellationToken);

        if (vinculo == null)
            return false;

        vinculo.Excluido = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}