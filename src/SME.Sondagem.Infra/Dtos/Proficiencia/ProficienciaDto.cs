using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Infra.Dtos.Proficiencia;

public class ProficienciaDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public int ComponenteCurricularId { get; set; }
    public int ModalidadeId { get; set; }
    public string? Modalidade { get; set; }
    public ICollection<QuestionarioDto> Questionarios { get; set; } = new List<QuestionarioDto>();
}