using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class PlanoAEEResumoIntegracaoDto
{
    public long Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Turma { get; set; } = string.Empty;
    public int Situacao { get; set; }
    public string CodigoAluno { get; set; } = string.Empty;
}
