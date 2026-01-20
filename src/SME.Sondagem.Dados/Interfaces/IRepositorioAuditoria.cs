using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Interfaces;

[ExcludeFromCodeCoverage]
public interface IRepositorioAuditoria : IRepositorioElasticBase<Dominio.Entidades.Auditoria>
{
    Task Salvar(Dominio.Entidades.Auditoria auditoria);
    Task SalvarMultiplos(IEnumerable<Dominio.Entidades.Auditoria> auditorias);
}