using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Infrastructure.Dtos
{
    public class TurmaCodigoElasticDto
    {
        public TurmaCodigoElasticDto(int? codigoTurma, string nomeTurma, string anoTurma)
        {
            CodigoTurma = codigoTurma;
            NomeTurma = nomeTurma;
            AnoTurma = anoTurma;
        }

        public int? CodigoTurma { get; set; }
        public string NomeTurma { get; set; }
        public string AnoTurma { get; set; }
    }
}
