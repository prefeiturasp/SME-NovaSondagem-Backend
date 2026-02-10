using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioParametroSondagemQuestionario : RepositorioBase<ParametroSondagemQuestionario>, IRepositorioParametroSondagemQuestionario
{
    public RepositorioParametroSondagemQuestionario(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase contexto) : base(
        context,
        servicoAuditoria, contexto)
    {
    }

    public async Task<IEnumerable<ParametroSondagemQuestionario>> ObterPorIdQuestionarioAsync(long idQuestionario, CancellationToken cancellationToken = default)
    {
        return await _context.ParametrosSondagemQuestionario
            .Include(p => p.ParametroSondagem)
            .Where(p => p.IdQuestionario == idQuestionario && !p.Excluido && !p.ParametroSondagem.Excluido)
            .ToListAsync(cancellationToken);
    }
}
