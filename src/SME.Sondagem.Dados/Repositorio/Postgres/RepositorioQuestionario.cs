using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioQuestionario(
    SondagemDbContext context,
    IServicoAuditoria servicoAuditoria,
    ContextoBase database)
    : RepositorioBase<Questionario>(context, servicoAuditoria, database), IRepositorioQuestionario;