using SME.Sondagem.Dados.Interfaces.Elastic;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioAuditoria : IRepositorioElasticBase<Dominio.Entidades.Auditoria>
{
    Task Salvar(Dominio.Entidades.Auditoria auditoria);
    Task SalvarMultiplos(IEnumerable<Dominio.Entidades.Auditoria> auditorias);
}