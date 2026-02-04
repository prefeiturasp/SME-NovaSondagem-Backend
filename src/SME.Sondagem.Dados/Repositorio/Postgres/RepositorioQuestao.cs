using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioQuestao : RepositorioBase<Questao>,IRepositorioQuestao
{

    public RepositorioQuestao(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase contextoBase) : base(context,
        servicoAuditoria, contextoBase)
    {
    }

    public async Task<IEnumerable<Questao>> ObterQuestionarioIdPorQuestoesAsync(IEnumerable<int> questoesId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Questoes
            .AsNoTracking()
            .Where(q => questoesId.Contains(q.Id) && !q.Excluido)
            .Include(q => q.Questionario)
            .Include(q => q.QuestaoOpcoes)
                .ThenInclude(qo => qo.OpcaoResposta)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Questao>> ObterQuestoesAtivasPorFiltroAsync(
    int modalidadeId,
    int anoLetivo,
    int proficienciaId,
    int serieAno,
    CancellationToken cancellationToken = default)
    {
        return await _context.Questoes
            .AsNoTracking()
            .Where(q => !q.Excluido 
                && q.Questionario.ModalidadeId == modalidadeId
                && q.Questionario.AnoLetivo == anoLetivo
                && q.Questionario.ProficienciaId == proficienciaId
                && q.Questionario.SerieAno == serieAno
                && !q.Questionario.Excluido)
            .Include(q => q.QuestaoVinculo)
            .Include(q => q.Questionario)
            .Include(q => q.QuestaoOpcoes)
                .ThenInclude(qo => qo.OpcaoResposta)
            .OrderBy(q => q.Ordem)
            .ToListAsync(cancellationToken);
    }

    public async Task<Questao?> ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(int questionarioId, TipoQuestao tipoQuestao, CancellationToken cancellationToken = default)
    {
        return await _context.Questoes
            .Include(q => q.Questionario)
            .Include(q => q.QuestaoOpcoes)
                .ThenInclude(qo => qo.OpcaoResposta)
            .FirstOrDefaultAsync(q => !q.Excluido 
            && q.QuestionarioId == questionarioId
            && q.Tipo == tipoQuestao, cancellationToken);
    }
}
