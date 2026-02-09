using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioSondagem : RepositorioBase<Dominio.Entidades.Sondagem.Sondagem>,IRepositorioSondagem
{
    public RepositorioSondagem(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase database) : base(context, servicoAuditoria, database)
    {
    }

    public async Task<Dominio.Entidades.Sondagem.Sondagem> ObterSondagemAtiva(CancellationToken cancellationToken = default)
    {
        var dataAtual = DateTimeExtension.HorarioBrasilia();
        var sondagem = await _context.Sondagens
            .AsNoTracking()
            .Where(s =>
                !s.Excluido &&
                s.PeriodosBimestre.Any(p =>
                    !p.Excluido &&
                    p.DataInicio <= dataAtual &&
                    p.DataFim >= dataAtual))
            .Include(s => s.PeriodosBimestre.Where(p => !p.Excluido))
            .ThenInclude(p => p.Bimestre)
            .FirstOrDefaultAsync(cancellationToken);

        return sondagem!;
    }
}