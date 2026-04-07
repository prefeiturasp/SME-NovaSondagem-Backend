using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres
{
    public class RepositorioRacaCor : RepositorioBase<RacaCor>, IRepositorioRacaCor
    {
        public RepositorioRacaCor(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase database) : base(context, servicoAuditoria, database)
        {
        }
    }
}
