using CsvHelper.Configuration.Attributes;

namespace SME.Sondagem.Infrastructure.Dtos
{
    public class ExtracaoSondagemLpEscritaDto
    {
        [Name("Nome DRE")]
        public string? NomeDre { get; set; }

        [Name("Código DRE")]
        public string? CodigoDre { get; set; }


        [Name("Código EOL Escola")]
        public string? CodigoEolEscola { get; set; }

        [Name("Nome Escola")]
        public string? NomeEscola { get; set; }

        [Name("Nome Turma")]
        public string? NomeTurma { get; set; }
        [Name("Código EOL Estudante")]
        public string? CodigoEolEstudante { get; set; }
        [Name("Nome Estudante")]
        public string? NomeEstudanteEstudante { get; set; }

        [Name("Questão")]
        public string? Questao { get; set; }

        [Name("Resposta")]
        public string? Resposta { get; set; }

        [Name("Legenda")]
        public string? Legenda { get; set; }

        [Name("Ano")]
        public string? Ano { get; set; }

        [Name("Bimestre")]
        public string? Bimestre { get; set; }

        [Name("Modalidade")]
        public string? Modalidade { get; set; }

        [Name("Modalidade Id")]
        public int? ModalidadeId { get; set; }

        [Name("Componente Curricular")]
        public string? ComponenteCurricular { get; set; }

        [Name("Proficiência")]
        public string? Proficiencia { get; set; }

    }
}