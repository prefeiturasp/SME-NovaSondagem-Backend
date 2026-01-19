using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos.Questionario;

[ExcludeFromCodeCoverage]
public class QuestionarioDto : BaseDto
{
    public required string Nome { get; set; }
    public TipoQuestionario Tipo { get; set; }
    public int AnoLetivo { get; set; }
    public int ComponenteCurricularId { get; set; }
    public int ProficienciaId { get; set; }
    public int SondagemId { get; set; }
    public int? ModalidadeId { get; set; }
    public int? SerieAno { get; set; }
}
