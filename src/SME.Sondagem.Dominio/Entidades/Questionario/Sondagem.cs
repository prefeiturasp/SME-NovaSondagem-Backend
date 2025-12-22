using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Dominio.Entidades.Questionario
{
    public class Sondagem : EntidadeBase
    {
        public Sondagem(int questionarioId, DateTime dataAplicacao)
        {
            QuestionarioId = questionarioId;
            DataAplicacao = dataAplicacao;
        }

        public int QuestionarioId { get; private set; }
        public DateTime DataAplicacao { get; private set; }

        // Navegação
        public virtual Questionario Questionario { get; private set; } = null!;
        public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();
    }
}