using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioQuestao : IRepositorioQuestao
{
    private readonly SondagemDbContext context;

    public RepositorioQuestao(SondagemDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Questao>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await context.Questoes
            .AsNoTracking()
            .Where(p => !p.Excluido)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Questao?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Questoes
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);
    }

    public async Task<long> CriarAsync(Questao questao, CancellationToken cancellationToken = default)
    {
        await context.Questoes.AddAsync(questao, cancellationToken);
        await context.SaveChangesAsync();
        return questao.Id;
    }

    public async Task<bool> AtualizarAsync(Questao questao, CancellationToken cancellationToken = default)
    {
        var questaoExistente = await context.Questoes
            .FirstOrDefaultAsync(p => p.Id == questao.Id && !p.Excluido, cancellationToken);

        if (questaoExistente == null)
            return false;

        questaoExistente.AlteradoEm = questao.AlteradoEm;
        questaoExistente.AlteradoPor = questao.AlteradoPor;
        questaoExistente.AlteradoRF = questao.AlteradoRF;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var questao = await context.Questoes
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);

        if (questao == null)
            return false;

        questao.Excluido = true;
        await context.SaveChangesAsync();
        return true;
    }
}
