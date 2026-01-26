namespace SME.Sondagem.Infrastructure.Dtos.Bimestre
{
    public class BimestreDto : BaseDto
    {
        public int CodBimestreEnsinoEol { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}