namespace SME.Sondagem.Infrastructure.Dtos.Ciclo
{
    public class CicloDto
    {
        public int Id { get; set; }
        public int CodCicloEnsinoEol { get; set; }
        public string DescCiclo { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public string CriadoRF { get; set; } = string.Empty;
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoRF { get; set; }
    }
}
