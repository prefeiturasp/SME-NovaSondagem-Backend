using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dominio.Entidades
{
    public class RacaCor : EntidadeBase
    {
        public required string Descricao { get; set; }
        public required int CodigoEolRacaCor { get; set; }
        public virtual ICollection<RespostaAluno> RespostaAlunos { get; private set; } = new List<RespostaAluno>();
    }
}
