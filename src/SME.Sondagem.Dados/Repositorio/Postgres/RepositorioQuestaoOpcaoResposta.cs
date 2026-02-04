using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioQuestaoOpcaoResposta : RepositorioBase<QuestaoOpcaoResposta>,IRepositorioQuestaoOpcaoResposta
{
    private readonly SondagemDbContext context;

    public RepositorioQuestaoOpcaoResposta(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase contextoBase) : base(context,
        servicoAuditoria, contextoBase)
    {
    }
}
