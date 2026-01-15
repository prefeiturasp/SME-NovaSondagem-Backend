namespace SME.Sondagem.Infrastructure.Dtos.Sondagem
{
    public class AlunoSondagemDto
    {
        public int Codigo { get; set; }  
        public string? NumeroEstudante { get; set; }
        public string? NomeEstudante { get; set; }
        public bool Lp { get; set; }
        public List<RespostaSondagemDto> Respostas { get; set; } = new();
    }
}
