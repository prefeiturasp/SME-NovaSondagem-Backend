namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class GrupoQuestoes : EntidadeBase
{
    public GrupoQuestoes(string titulo, string? subtitulo)
    {
        Titulo = titulo;
        Subtitulo = subtitulo;
    }

    public string Titulo { get; private set; }
    public string? Subtitulo { get; private set; }

    // Navegação
    public virtual ICollection<Questao> Questoes { get; private set; } = new List<Questao>();
}