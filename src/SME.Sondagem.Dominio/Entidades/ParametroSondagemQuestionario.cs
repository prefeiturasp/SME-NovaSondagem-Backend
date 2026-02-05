using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dominio.Entidades
{
    [ExcludeFromCodeCoverage]
    public class ParametroSondagemQuestionario : EntidadeBase
    {
        public ParametroSondagemQuestionario()
        {
        }

        public ParametroSondagemQuestionario(int idParametroSondagem, int idQuestionario, string valor, string criadoPor, string criadoRF)
        {
            IdParametroSondagem = idParametroSondagem;
            IdQuestionario = idQuestionario;
            Valor = valor;
            CriadoPor = criadoPor;
            CriadoRF = criadoRF;
        }

        public int IdQuestionario { get; set; }
        public int IdParametroSondagem { get; set; }
        public string? Valor { get; set; }

        public virtual Questionario.Questionario Questionario { get; set; } = null!;
        public virtual ParametroSondagem ParametroSondagem { get; set; } = null!;

        public void Atualizar(int idParametroSondagem, int idQuestionario, string? valor)
        {
            IdParametroSondagem = idParametroSondagem;
            IdQuestionario = idQuestionario;
            Valor = valor;
        }
    }
}
