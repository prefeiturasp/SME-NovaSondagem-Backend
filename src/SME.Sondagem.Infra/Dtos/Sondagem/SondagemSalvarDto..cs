namespace SME.Sondagem.Infrastructure.Dtos.Sondagem
{
    public class SondagemSalvarDto
    {
        public int SondagemId { get; set; }
        public int TurmaId { get; set; }
        public int UeId { get; set; }
        public int DreId { get; set; }
        public int AnoLetivo { get; set; }
        public int ModalidadeId { get; set; }
        public List<AlunoSondagemDto> Alunos { get; set; } = new();
    }
}
