namespace SME.Sondagem.Dominio.Entidades;

public class Auditoria
{
    public Auditoria(string? acao, long chave, DateTime data, string? entidade, Guid? id, string? rF, string? usuario, Guid? perfil, string? administrador)
    {
        Acao = acao;
        Chave = chave;
        Data = data;
        Entidade = entidade;
        Id = id;
        RF = rF;
        Usuario = usuario;
        Perfil = perfil;
        Administrador = administrador;
        Detalhes = new List<AuditoriaDetalhe>();
    }

    public string? Acao { get; private set; }
    public long Chave { get; private set; }
    public DateTime Data { get; private set; }
    public string? Entidade { get; private set; }
    public Guid? Id { get; private set; }
    public string? RF { get; private set; }
    public string? Usuario { get; private set; }
    public Guid? Perfil { get; private set; }
    public string? Administrador { get; private set; }

    // Navegação para detalhes
    public ICollection<AuditoriaDetalhe> Detalhes { get; private set; }
}