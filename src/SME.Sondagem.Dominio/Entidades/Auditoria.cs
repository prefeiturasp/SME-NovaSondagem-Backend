namespace SME.Sondagem.Dominio.Entidades;

public class Auditoria
{
    public required long Chave { get; init; }
    public required DateTime Data { get; init; }
    public string? Acao { get; init; }
    public string? Entidade { get; init; }
    public Guid? Id { get; init; }
    public string? RF { get; init; }
    public string? Usuario { get; init; }
    public Guid? Perfil { get; init; }
    public string? Administrador { get; init; }

    // Navegação para detalhes
    public ICollection<AuditoriaDetalhe> Detalhes { get; private set; } = new List<AuditoriaDetalhe>();
}