using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioOpcaoResposta : IRepositorioOpcaoResposta
{
    private readonly SondagemDbContext context;

    public RepositorioOpcaoResposta(SondagemDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<OpcaoResposta>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await context.OpcoesResposta
            .AsNoTracking()
            .Where(p => !p.Excluido)
            .OrderBy(p => p.DescricaoOpcaoResposta)
            .ToListAsync(cancellationToken);
    }

    public async Task<OpcaoResposta?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.OpcoesResposta
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);
    }

    public async Task<long> CriarAsync(OpcaoResposta opcaoResposta, CancellationToken cancellationToken = default)
    {
        await context.OpcoesResposta.AddAsync(opcaoResposta, cancellationToken);
        await context.SaveChangesAsync();
        return opcaoResposta.Id;
    }

    public async Task<bool> AtualizarAsync(OpcaoResposta opcaoResposta, CancellationToken cancellationToken = default)
    {
        var opcaoRespostaExistente = await context.OpcoesResposta
            .FirstOrDefaultAsync(p => p.Id == opcaoResposta.Id && !p.Excluido, cancellationToken);

        if (opcaoRespostaExistente == null)
            return false;

        opcaoRespostaExistente.AlteradoEm = opcaoResposta.AlteradoEm;
        opcaoRespostaExistente.AlteradoPor = opcaoResposta.AlteradoPor;
        opcaoRespostaExistente.AlteradoRF = opcaoResposta.AlteradoRF;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var opcaoResposta = await context.OpcoesResposta
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido, cancellationToken);

        if (opcaoResposta == null)
            return false;

        opcaoResposta.Excluido = true;
        await context.SaveChangesAsync();
        return true;
    }
}
