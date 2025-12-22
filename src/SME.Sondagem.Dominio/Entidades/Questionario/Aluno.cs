namespace SME.Sondagem.Dominio.Entidades.Questionario
{
    public class Aluno : EntidadeBase
    {
        public Aluno(string? raAluno, string? nomeAluno, bool isPap, bool isAee, bool isPcd, int? deficienciaId, string? deficienciaNome, int? racaId, string? racaNome, int? corId, string? corNome)
        {
            RaAluno = raAluno;
            NomeAluno = nomeAluno;
            IsPap = isPap;
            IsAee = isAee;
            IsPcd = isPcd;
            DeficienciaId = deficienciaId;
            DeficienciaNome = deficienciaNome;
            RacaId = racaId;
            RacaNome = racaNome;
            CorId = corId;
            CorNome = corNome;
        }

        public string? RaAluno { get; private set; }
        public string? NomeAluno { get; private set; }
        public bool IsPap { get; private set; }
        public bool IsAee { get; private set; }
        public bool IsPcd { get; private set; }
        public int? DeficienciaId { get; private set; }
        public string? DeficienciaNome { get; private set; }
        public int? RacaId { get; private set; }
        public string? RacaNome { get; private set; }
        public int? CorId { get; private set; }
        public string? CorNome { get; private set; }

        // Navegação
        public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();
    }
}