namespace SME.Sondagem.Infrastructure.Dtos.Sondagem
{
    public class SondagemSalvarDto
    {
        public int SondagemId { get; set; }
        public string? TurmaId { get; set; }
        public string? UeId { get; set; }
        public string? DreId { get; set; }
        public int AnoLetivo { get; set; }
        public int? AnoTurma { get; set; }
        public int? ModalidadeId { get; set; }
        public List<AlunoSondagemDto> Alunos { get; set; } = new();
    }
}
