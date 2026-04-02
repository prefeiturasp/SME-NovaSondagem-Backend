namespace SME.Sondagem.Infrastructure.Dtos
{
    public class AlunoRacaGeneroDto
    {
        public long CodigoAluno { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Raca { get; set; } = string.Empty;
    }
}
