using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class QuestaoDto : BaseDto
{
    public int QuestionarioId { get; set; }
    public int? GrupoQuestoesId { get; set; }
    public int Ordem { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;
    public bool Obrigatorio { get; set; }
    public TipoQuestao Tipo { get; set; }
    public string Opcionais { get; set; } = string.Empty;
    public bool SomenteLeitura { get; set; }
    public int Dimensao { get; set; }
    public int? Tamanho { get; set; }
    public string? Mascara { get; set; }
    public string? PlaceHolder { get; set; }
    public string? NomeComponente { get; set; }
}
