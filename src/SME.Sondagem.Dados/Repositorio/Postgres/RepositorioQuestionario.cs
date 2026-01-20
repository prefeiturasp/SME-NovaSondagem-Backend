using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioQuestionario : IRepositorioQuestionario
{
    private readonly SondagemDbContext context;

    public RepositorioQuestionario(SondagemDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Questionario>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await context.Questionarios
            .AsNoTracking()
            .Where(p => !p.Excluido)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }   

    public async Task<Questionario?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Questionarios
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);
    }

    public async Task<long> CriarAsync(Questionario questionario, CancellationToken cancellationToken = default)
    {
        await context.Questionarios.AddAsync(questionario, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return questionario.Id;
    }

    public async Task<bool> AtualizarAsync(Questionario questionario, CancellationToken cancellationToken = default)
    {
        var questionarioExistente = await context.Questionarios
            .FirstOrDefaultAsync(p => p.Id == questionario.Id && !p.Excluido, cancellationToken);

        if (questionarioExistente == null)
            return false;

        questionarioExistente.AlteradoEm = questionario.AlteradoEm;
        questionarioExistente.AlteradoPor = questionario.AlteradoPor;
        questionarioExistente.AlteradoRF = questionario.AlteradoRF;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var questionario = await context.Questionarios
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);

        if (questionario == null)
            return false;

        questionario.Excluido = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}