namespace SME.Sondagem.Dados.Interfaces.Auditoria;

public interface IServicoAuditoria
{
    Task AuditarAsync(string entidade, long chave, string acao);
    Task AuditarMultiplosAsync(string entidade, IEnumerable<long> chaves, string acao);
}