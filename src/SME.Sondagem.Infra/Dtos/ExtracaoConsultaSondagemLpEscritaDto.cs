namespace SME.Sondagem.Infrastructure.Dtos
{
    public class ExtracaoConsultaSondagemLpEscritaDto
    {
        public string? NomeDre { get; set; }

        public string? CodigoDre { get; set; }


        public string? CodigoEolEscola { get; set; }

        public string? NomeEscola { get; set; }
        public string? TurmaId { get; set; }

        public int? AnoTurma { get; set; }
        public string? NomeTurma { get; set; }
        
        public string? CodigoEolEstudante { get; set; }
        
        public string? NomeEstudanteEstudante { get; set; }

        
        public string? Questao { get; set; }

        
        public string? Resposta { get; set; }

        
        public string? Legenda { get; set; }

        
        public string? Ano { get; set; }

        
        public string? Bimestre { get; set; }

        
        public string? Modalidade { get; set; }

        
        public int? ModalidadeId { get; set; }

        
        public string? ComponenteCurricular { get; set; }

        
        public string? Proficiencia { get; set; }
        public int? RacaId { get; set; }
        public int? GeneroId { get; set; }
    }
}
