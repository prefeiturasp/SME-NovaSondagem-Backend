namespace SME.Sondagem.Infrastructure.Dtos
{
    public class AlunoEolDto
    {
        public int CodigoAluno { get; set; }   
        public int codigoTurma { get; set; }   
        public string NomeAluno { private get; set; }   
        public string NomeSocialAluno { get; set; }
        public string NomeResponsavel { get; set; }

        public string NomeCompleto()
        {
            return NomeSocialAluno ?? NomeAluno;
        }
    }
}