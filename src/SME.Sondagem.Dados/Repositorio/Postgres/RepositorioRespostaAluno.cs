using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
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

    public async Task<Dictionary<(int AlunoId, int CicloId), (int Id, int OpcaoRespostaId)>> ObterRespostasAlunosPorCiclosAsync(
        List<int> alunosIds, 
        List<int> ciclosIds, 
        int proficienciaId, 
        int ano, 
        CancellationToken cancellationToken)
    {
        var respostas = await _context.RespostasAluno
            .Include(ra => ra.Sondagem)
                .ThenInclude(s => s.Questionario)
                    .ThenInclude(q => q.Ciclo)
            .Include(ra => ra.Questao)
            .Where(ra => alunosIds.Contains(ra.AlunoId) 
                && ciclosIds.Contains(ra.Sondagem.Questionario.CicloId)
                && ra.Sondagem.Questionario.ProficienciaId == proficienciaId)
            .Select(ra => new
            {
                ra.AlunoId,
                CicloId = ra.Sondagem.Questionario.CicloId,
                ra.Id,
                ra.OpcaoRespostaId
            })
            .ToListAsync(cancellationToken);

        return respostas.ToDictionary(
            r => (r.AlunoId, r.CicloId),
            r => (r.Id, r.OpcaoRespostaId)
        );
    }
}