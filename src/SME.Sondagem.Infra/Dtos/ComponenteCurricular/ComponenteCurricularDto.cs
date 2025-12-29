namespace SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular
{
    public class ComponenteCurricularDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int? Ano { get; set; }
        public string? Modalidade { get; set; }
        public int CodigoEol { get; set; }
        public DateTime CriadoEm { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
    }
}