namespace SME.Sondagem.Dominio.ValueObjects
{
    public record ContextoEducacional
    {
        public string? TurmaId { get; init; }
        public string? UeId { get; init; }
        public string? DreId { get; init; }
        public int? AnoLetivo { get; init; }
        public string? ModalidadeId { get; init; }
        public int? RacaCorId { get; init; }
        public int? GeneroSexoId { get; init; }
        public int? BimestreId { get; init; }
        public int? ProgramaAtendimentoId { get; set; }
    }
}
