namespace SME.Sondagem.Infrastructure.Dtos.Relatorio
{
    public class DadosAlunoPorTurmaDTO
    {
        public long NumeroChamada { get; set; }
        public long CodigoAluno { get; set; }
        public required string NomeAluno { get; set; }
        public string NomeSocialAluno { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string Raca { get; set; } = string.Empty;
    }
}
