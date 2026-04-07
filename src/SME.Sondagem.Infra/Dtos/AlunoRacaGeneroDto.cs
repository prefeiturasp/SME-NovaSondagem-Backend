namespace SME.Sondagem.Infrastructure.Dtos
{
    public class AlunoRacaGeneroDto
    {
        public long CodigoAluno { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public int? SexoId { get; set; }
        public string Raca { get; set; } = string.Empty;
        public int? RacaId { get; set; }
    }
}
