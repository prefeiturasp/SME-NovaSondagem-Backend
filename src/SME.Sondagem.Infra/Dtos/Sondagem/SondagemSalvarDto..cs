namespace SME.Sondagem.Infrastructure.Dtos.Sondagem
{
    public class SondagemSalvarDto
    {
        public int SondagemId { get; set; }
        public List<AlunoSondagemDto> Alunos { get; set; } = new();
    }
}
