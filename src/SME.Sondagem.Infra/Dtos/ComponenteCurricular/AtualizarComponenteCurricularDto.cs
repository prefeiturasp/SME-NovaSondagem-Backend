namespace SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular
{
    public class AtualizarComponenteCurricularDto
    {
        public string Nome { get; set; } = string.Empty;
        public int? Ano { get; set; }
        public string? Modalidade { get; set; }
        public int CodigoEol { get; set; }
    }
}