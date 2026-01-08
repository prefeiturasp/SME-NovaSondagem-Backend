using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class EstudanteQuestionarioDto
{
    public string NumeroAlunoChamada { get; set; }
    public bool LinguaPortuguesaSegundaLingua { get; set; }
    public int Codigo { get; set; }
    public string Nome { get; set; }
    public bool Pap { get; set; }
    public bool Aee { get; set; }
    public bool PossuiDeficiencia { get; set; }
    public IEnumerable<ColunaQuestionarioDto> Coluna { get; set; }
}