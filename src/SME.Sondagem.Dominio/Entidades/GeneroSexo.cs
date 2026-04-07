using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dominio.Entidades
{
    public class GeneroSexo : EntidadeBase
    {
        public required string Descricao { get; set; }
        public string? Sigla { get; set; }
        public virtual ICollection<RespostaAluno> RespostaAlunos { get; private set; } = new List<RespostaAluno>();
    }
}
