using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

[ExcludeFromCodeCoverage]
public class UsuarioLogadoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Rf { get; set; } = string.Empty;
}