using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Dominio.Entidades.Questionario
{
    public class ComponenteCurricular : EntidadeBase
    {
        public ComponenteCurricular(string nome, int? ano, string? modalidade)
        {
            Nome = nome;
            Ano = ano;
            Modalidade = modalidade;
        }

        public string Nome { get; private set; } = string.Empty;
        public int? Ano { get; private set; }
        public string? Modalidade { get; private set; }

        // Navegação
        public virtual ICollection<Proficiencia> Proficiencias { get; private set; } = new List<Proficiencia>();
        public virtual ICollection<Questionario> Questionarios { get; private set; } = new List<Questionario>();
    }
}