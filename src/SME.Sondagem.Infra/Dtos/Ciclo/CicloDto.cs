namespace SME.Sondagem.Infrastructure.Dtos.Ciclo
{
    public class CicloDto : BaseDto
    {
        public int Id { get; set; }
        public int CodCicloEnsinoEol { get; set; }
        public string DescCiclo { get; set; } = string.Empty;
    }
}
