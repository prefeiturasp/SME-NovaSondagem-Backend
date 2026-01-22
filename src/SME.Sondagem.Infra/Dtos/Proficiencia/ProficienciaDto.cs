using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Infra.Dtos.Proficiencia;

public class ProficienciaDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public int ComponenteCurricularId { get; set; }
}