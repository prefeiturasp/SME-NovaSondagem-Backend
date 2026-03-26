using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Infrastructure.Dtos
{
    public class TurmaCodigoElasticDto
    {
        public TurmaCodigoElasticDto(int? codigoTurma, string nomeTurma, string anoTurma, int? codigoAluno, string codigoEscola)
        {
            CodigoTurma = codigoTurma;
            NomeTurma = nomeTurma;
            AnoTurma = anoTurma;
            CodigoAluno = codigoAluno;
            CodigoEscola = codigoEscola;
        }

        public int? CodigoTurma { get; set; }
        public int? CodigoAluno { get; set; }
        public string NomeTurma { get; set; }
        public string AnoTurma { get; set; }
        public string CodigoEscola { get; set; }
        public DateTime DataMatricula { get; set; }
    }
}
