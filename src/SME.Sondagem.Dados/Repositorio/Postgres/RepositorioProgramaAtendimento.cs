using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres
{
    public class RepositorioProgramaAtendimento : RepositorioBase<ProgramaAtendimento>, IRepositorioProgramaAtendimento
    {
        public RepositorioProgramaAtendimento(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase database) : base(context, servicoAuditoria, database)
        {
        }
    }
}
