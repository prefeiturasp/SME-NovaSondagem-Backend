using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioRespostaAluno : RepositorioBase<RespostaAluno>, IRepositorioRespostaAluno
{
    public RepositorioRespostaAluno(SondagemDbContext context, IServicoAuditoria servicoAuditoria) : base(context,
        servicoAuditoria)
    {
    }

    public async Task<bool> VerificarAlunoTemRespostaPorTipoQuestaoAsync(int alunoId, TipoQuestao tipoQuestao,
        CancellationToken cancellationToken)
    {
        return await _context.RespostasAluno
            .AnyAsync(ra => ra.AlunoId == alunoId && ra.Questao.Tipo == tipoQuestao, cancellationToken);
    }

    public async Task<Dictionary<int, bool>> VerificarAlunosTemRespostaPorTipoQuestaoAsync(List<int> alunosIds,
        TipoQuestao tipoQuestao, CancellationToken cancellationToken)
    {
        var respostas = await _context.RespostasAluno
            .Include(ra => ra.Questao)
            .Where(ra => ra.AlunoId.HasValue && alunosIds.Contains(ra.AlunoId.Value) && ra.Questao.Tipo == tipoQuestao)
            .Select(ra => ra.AlunoId ?? 0) 
            .Distinct()
            .ToListAsync(cancellationToken);

        return alunosIds
            .Distinct()
            .ToDictionary(
                alunoId => alunoId,
                alunoId => respostas.Contains(alunoId)
            );
    }


    public async Task<IEnumerable<RespostaAluno>> ObterRespostasPorSondagemEAlunosAsync(int sondagemId,
        IEnumerable<int> alunosIds, IEnumerable<int> questoesIds,
        CancellationToken cancellationToken = default)
    {
        var alunosIdsList = alunosIds.ToList();
        var questoesIdsList = questoesIds.ToList();

        if (alunosIdsList.Count == 0 || questoesIdsList.Count == 0)
            return [];

        return await _context.RespostasAluno
            .AsNoTracking()
            .Where(ra => !ra.Excluido && ra.SondagemId == sondagemId
                                  && ra.AlunoId.HasValue && alunosIdsList.Contains(ra.AlunoId.Value)
                                  && questoesIdsList.Contains(ra.QuestaoId))
            .ToListAsync(cancellationToken);
    }


    public async Task<Dictionary<(long CodigoAluno, long QuestaoId, int? BimestreId), RespostaAluno>>
        ObterRespostasAlunosPorQuestoesAsync(
            List<long> codigosAlunos,
            List<long> questoesIds,
            long sondagemId,
            CancellationToken cancellationToken = default)
    {
        var respostas = await _context.RespostasAluno
            .Where(r => r.AlunoId.HasValue
                        && codigosAlunos.Contains((long)r.AlunoId.Value)
                        && questoesIds.Contains(r.QuestaoId)
                        && r.SondagemId == sondagemId
                        && !r.Excluido)
            .ToListAsync(cancellationToken);

        return respostas.ToDictionary(
            r => ((long)(r.AlunoId ?? 0), (long)r.QuestaoId, r.BimestreId)
        );
    }
}