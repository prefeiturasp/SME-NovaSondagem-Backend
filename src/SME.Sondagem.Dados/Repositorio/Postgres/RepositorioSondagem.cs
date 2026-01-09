using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioSondagem : IRepositorioSondagem
{
    private readonly SondagemDbContext _context;
    public RepositorioSondagem(SondagemDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task InserirAsync(Dominio.Entidades.Sondagem.Sondagem entidade, CancellationToken cancellationToken = default)
    {
        _context.Sondagens.Add(entidade);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Dominio.Entidades.Sondagem.Sondagem> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sondagem = await _context.Sondagens
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return sondagem!;
    }

    public async Task<IEnumerable<Dominio.Entidades.Sondagem.Sondagem>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sondagens.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Dominio.Entidades.Sondagem.Sondagem> ObterSondagemAtiva(CancellationToken cancellationToken = default)
    {
        var dataAtual = DateTime.UtcNow;
        var sondagem = await _context.Sondagens
            .AsNoTracking()
            .Where(s =>
                !s.Excluido &&
                s.PeriodosBimestre.Any(p =>
                    !p.Excluido &&
                    p.DataInicio <= dataAtual &&
                    p.DataFim >= dataAtual))
            .Include(s => s.PeriodosBimestre.Where(p => !p.Excluido))
            .FirstOrDefaultAsync(cancellationToken);

        return sondagem!;
    }
}