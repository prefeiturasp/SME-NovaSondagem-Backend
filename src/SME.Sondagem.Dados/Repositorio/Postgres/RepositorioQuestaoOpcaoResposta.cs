using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioQuestaoOpcaoResposta : IRepositorioQuestaoOpcaoResposta
{
    private readonly SondagemDbContext context;

    public RepositorioQuestaoOpcaoResposta(SondagemDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<QuestaoOpcaoResposta>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await context.QuestoesOpcoesResposta
            .AsNoTracking()
            .Where(p => !p.Excluido)
            .OrderBy(p => p.Ordem)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuestaoOpcaoResposta?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.QuestoesOpcoesResposta
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);
    }

    public async Task<long> CriarAsync(QuestaoOpcaoResposta questaoOpcaoResposta, CancellationToken cancellationToken = default)
    {
        await context.QuestoesOpcoesResposta.AddAsync(questaoOpcaoResposta, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return questaoOpcaoResposta.Id;
    }

    public async Task<bool> AtualizarAsync(QuestaoOpcaoResposta questaoOpcaoResposta, CancellationToken cancellationToken = default)
    {
        var questaoOpcaoRespostaExistente = await context.QuestoesOpcoesResposta
            .FirstOrDefaultAsync(p => p.Id == questaoOpcaoResposta.Id && !p.Excluido, cancellationToken);

        if (questaoOpcaoRespostaExistente == null)
            return false;

        questaoOpcaoRespostaExistente.AlteradoEm = questaoOpcaoResposta.AlteradoEm;
        questaoOpcaoRespostaExistente.AlteradoPor = questaoOpcaoResposta.AlteradoPor;
        questaoOpcaoRespostaExistente.AlteradoRF = questaoOpcaoResposta.AlteradoRF;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var questaoOpcaoResposta = await context.QuestoesOpcoesResposta
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);

        if (questaoOpcaoResposta == null)
            return false;

        questaoOpcaoResposta.Excluido = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
