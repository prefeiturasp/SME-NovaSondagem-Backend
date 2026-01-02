namespace SME.Sondagem.Infrastructure.Dtos
{
    public class BaseDto
    {
        public DateTime CriadoEm { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public string CriadoRF { get; set; } = string.Empty;
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoRF { get; set; }
    }
}
