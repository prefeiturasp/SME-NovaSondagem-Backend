using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Dominio.Entidades.Questionario
{
    public class Proficiencia : EntidadeBase
    {
        public Proficiencia(string nome, int componenteCurricularId)
        {
            Nome = nome;
            ComponenteCurricularId = componenteCurricularId;
        }

        public string Nome { get; private set; } = string.Empty;
        public int ComponenteCurricularId { get; private set; }

        // Navegação
        public virtual ComponenteCurricular ComponenteCurricular { get; private set; } = null!;
        public virtual ICollection<Questionario> Questionarios { get; private set; } = new List<Questionario>();
    }
}