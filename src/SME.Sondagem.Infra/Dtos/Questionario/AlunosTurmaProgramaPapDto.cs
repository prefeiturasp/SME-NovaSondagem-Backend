using SME.Sondagem.Dominio.Enums;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    [ExcludeFromCodeCoverage]
    public class DadosMatriculaAlunoTipoPapDto
    {
        public int AnoLetivo { get; set; }
        public int CodigoTurma { get; set; }
        public string CodigoDre { get; set; } = string.Empty;
        public string CodigoUe { get; set; } = string.Empty;
        public int CodigoAluno { get; set; }
        public int ComponenteCurricularId { get; set; }
        public TipoPap TipoPap { get; set; }
    }
}
