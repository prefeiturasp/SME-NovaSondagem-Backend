using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos
{
    [ExcludeFromCodeCoverage]
    public class BaseDto
    {
        public int? Id { get; set; }
        public DateTime CriadoEm { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public string CriadoRF { get; set; } = string.Empty;
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoRF { get; set; }
    }
}
