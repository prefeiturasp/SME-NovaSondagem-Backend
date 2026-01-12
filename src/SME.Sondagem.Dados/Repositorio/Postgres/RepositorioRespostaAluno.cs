using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using Microsoft.EntityFrameworkCore;

namespace SME.Sondagem.Dados.Repositorio.Postgres
{
    public class RepositorioRespostaAluno : RepositorioBase<RespostaAluno>, IRepositorioRespostaAluno
    {
        public RepositorioRespostaAluno(SondagemDbContext context) : base(context) { }

        public async Task<IEnumerable<RespostaAluno>> ObterRespostasPorSondagemEAlunosAsync(int sondagemId, IEnumerable<int> alunosIds, IEnumerable<int> questoesIds,
            CancellationToken cancellationToken = default)
        {
            var alunosIdsList = alunosIds.ToList();
            var questoesIdsList = questoesIds.ToList();
            
            if (alunosIdsList.Count == 0 || questoesIdsList.Count == 0)
                return [];
            
            return await _context.RespostasAluno
                .AsNoTracking() 
                .Where(ra => !ra.Excluido && ra.SondagemId == sondagemId 
                             && alunosIdsList.Contains(ra.AlunoId)
                             && questoesIdsList.Contains(ra.QuestaoId))
                .ToListAsync(cancellationToken);
        }
    }
}
