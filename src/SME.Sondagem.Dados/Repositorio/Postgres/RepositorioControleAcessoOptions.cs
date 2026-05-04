using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades.Configuration;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres
{
    public class RepositorioControleAcessoOptions : RepositorioBase<ControleAcessoOptions>, IRepositorioControleAcessoOptions
    {
        public RepositorioControleAcessoOptions(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase database) : base(context, servicoAuditoria, database)
        {
        }

        public async Task<IEnumerable<ControleAcessoOptions>> ObterTodosComPerfis()
        {
           return await _context.ControleAcessoOptions.AsNoTracking().Where(x => !x.Excluido).Include(x => x.ConfiguracaoPerfis).ToListAsync();
        }
    }
}
