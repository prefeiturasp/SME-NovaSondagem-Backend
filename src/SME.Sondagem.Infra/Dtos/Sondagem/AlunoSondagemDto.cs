namespace SME.Sondagem.Infrastructure.Dtos.Sondagem
{
    public class AlunoSondagemDto
    {
        public int AlunoId { get; set; }  
        public string NumeroEstudante { get; set; } = null!;
        public string NomeEstudante { get; set; } = null!;
        public bool Lp { get; set; }
        public List<RespostaSondagemDto> Respostas { get; set; } = new();
    }
}
