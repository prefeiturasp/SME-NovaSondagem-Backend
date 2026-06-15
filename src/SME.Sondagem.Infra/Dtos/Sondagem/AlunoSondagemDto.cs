namespace SME.Sondagem.Infrastructure.Dtos.Sondagem
{
    public class AlunoSondagemDto
    {
        public int Codigo { get; set; }
        public string NumeroAlunoChamada { get; set; } = null!;
        public string NomeEstudante { get; set; } = null!;
        public bool LinguaPortuguesaSegundaLingua { get; set; }
        public int? ProgramaAtendimentoId { get; set; }
        public bool Pap { get; set; }
        public bool Aee { get; set; }
        public bool Deficiente { get; set; }
        public List<RespostaSondagemDto> Respostas { get; set; } = new();
    }
}
