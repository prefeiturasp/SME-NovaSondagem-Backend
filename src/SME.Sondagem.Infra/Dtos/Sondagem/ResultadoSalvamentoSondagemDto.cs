using System.Text.Json.Serialization;

namespace SME.Sondagem.Infrastructure.Dtos.Sondagem;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultadoSalvamentoSondagem
{
    Gravado,
    SemAlteracoes,
    NaoProcessado
}

public class ResultadoSalvamentoSondagemDto
{
    public ResultadoSalvamentoSondagem Resultado { get; init; }
    public int QuantidadeEstudantesNaoProcessados { get; init; }
}
