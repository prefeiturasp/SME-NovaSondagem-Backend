namespace SME.Sondagem.Dominio.Entidades;

public class AuditoriaDetalhe
{
    public AuditoriaDetalhe(Guid auditoriaId, string nomePropriedade, string? valorAntigo, string? valorNovo)
    {
        Id = Guid.NewGuid();
        AuditoriaId = auditoriaId;
        NomePropriedade = nomePropriedade;
        ValorAntigo = valorAntigo;
        ValorNovo = valorNovo;
        Auditoria = null!;
    }

    public Guid Id { get; private set; }
    public Guid AuditoriaId { get; private set; }
    public string NomePropriedade { get; private set; }
    public string? ValorAntigo { get; private set; }
    public string? ValorNovo { get; private set; }

    // Navegação
    public Auditoria Auditoria { get; private set; }
}