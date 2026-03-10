namespace SME.Sondagem.Infrastructure.Dtos
{
    public class AlunoEolDto
    {
        public string? NomeAluno { get; set; }
        public string? SituacaoMatricula { get; set; }
        public string? CodigoEscola { get; set; }
        public DateTime DataMatricula { get; set; }
        public int CodigoAluno { get; set; }
        public int CodigoTurma { get; set; }
        public int CodigoSituacaoMatricula { get; set; }

    }
}