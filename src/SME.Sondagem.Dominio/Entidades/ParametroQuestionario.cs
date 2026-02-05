using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dominio.Entidades
{
    [ExcludeFromCodeCoverage]
    public class ParametroQuestionario : EntidadeBase
    {
        public int IdQuestionario { get; set; }
        public int IdParametroSondagem { get; set; }

        public virtual Questionario.Questionario Questionario { get; set; } = null!;
        public virtual ParametroSondagem ParametroSondagem { get; set; } = null!;
    }
}
