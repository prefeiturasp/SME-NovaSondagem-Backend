using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Infrastructure.Dtos
{
    public class TurmaCodigoElasticDto
    {
        public TurmaCodigoElasticDto(int? codigoTurma, string nomeTurma)
        {
            CodigoTurma = codigoTurma ?? throw new ArgumentNullException(nameof(codigoTurma));
            NomeTurma = nomeTurma ?? throw new ArgumentNullException(nameof(nomeTurma));
        }

        public int CodigoTurma { get; set; }
        public string NomeTurma { get; set; }
    }
}
