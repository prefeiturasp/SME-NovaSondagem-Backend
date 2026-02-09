using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioOpcaoResposta : RepositorioBase<OpcaoResposta>, IRepositorioOpcaoResposta
{

    public RepositorioOpcaoResposta(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase contexto) : base(
        context,
        servicoAuditoria,contexto)
    {
    }
}
