namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public class DadosAlunosDto
    {
        public required Dictionary<int, bool> AlunosComPap { get; set; }
        public required Dictionary<int, bool> AlunosComLinguaPortuguesaSegundaLingua { get; set; }
        public Dictionary<long, (string Raca, string Sexo)>? DadosRacaGenero { get; set; }
    }
}
