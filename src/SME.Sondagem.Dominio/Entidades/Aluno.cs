using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dominio.Entidades;

public class Aluno : EntidadeBase
{
    public required bool IsPap { get; init; }
    public required bool IsAee { get; init; }
    public required bool IsPcd { get; init; }
    public string? RaAluno { get; init; }
    public string? NomeAluno { get; init; }
    public int? DeficienciaId { get; init; }
    public string? DeficienciaNome { get; init; }
    public int? RacaId { get; init; }
    public string? RacaNome { get; init; }
    public int? CorId { get; init; }
    public string? CorNome { get; init; }

    // Navegação
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();
}