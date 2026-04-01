namespace SME.Sondagem.Dominio.ValueObjects
{
    public record ContextoEducacional
    {
        public string? TurmaId { get; init; }
        public string? UeId { get; init; }
        public string? DreId { get; init; }
        public int? AnoLetivo { get; init; }
        public string? ModalidadeId { get; init; }
        public string? Raca { get; init; }
        public string? Genero { get; init; }
        public int? BimestreId { get; init; }
    }
}
