using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioRespostaAluno : IRepositorioRespostaAluno
{
    private readonly SondagemDbContext _context;

    public RepositorioRespostaAluno(SondagemDbContext context)
    {
        _context = context;
    }

    public async Task<bool> VerificarAlunoTemRespostaPorTipoQuestaoAsync(int alunoId, TipoQuestao tipoQuestao, CancellationToken cancellationToken)
    {
        return await _context.RespostasAluno
            .AnyAsync(ra => ra.AlunoId == alunoId && ra.Questao.Tipo == tipoQuestao, cancellationToken);
    }

    public async Task<Dictionary<int, bool>> VerificarAlunosTemRespostaPorTipoQuestaoAsync(List<int> alunosIds, TipoQuestao tipoQuestao, CancellationToken cancellationToken)
    {
        var respostas = await _context.RespostasAluno
            .Include(ra => ra.Questao)
            .Where(ra => alunosIds.Contains(ra.AlunoId) && ra.Questao.Tipo == tipoQuestao)
            .Select(ra => ra.AlunoId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return alunosIds.ToDictionary(
            alunoId => alunoId,
            alunoId => respostas.Contains(alunoId)
        );
    }

    public async Task<Dictionary<(long CodigoAluno, long QuestaoId), RespostaAluno>> ObterRespostasAlunosPorQuestoesAsync(
    List<long> codigosAlunos,
    List<long> questoesIds,
    long sondagemId,
    CancellationToken cancellationToken = default)
    {
        var respostas = await _context.RespostasAluno
            .Where(r => codigosAlunos.Contains(r.AlunoId)
                && questoesIds.Contains(r.QuestaoId)
                && r.SondagemId == sondagemId
                && !r.Excluido)
            .ToListAsync(cancellationToken);

        return respostas.ToDictionary(
            r => ((long)r.AlunoId, (long)r.QuestaoId)
        );
    }
}