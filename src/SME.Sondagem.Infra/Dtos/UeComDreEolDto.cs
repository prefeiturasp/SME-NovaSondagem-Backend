using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Infrastructure.Dtos
{
    public class UeComDreEolDto
    {
        public UeComDreEolDto()
        {

        }
        public UeComDreEolDto(string codigoEscola, string nomeEscola, string nomeDRE, string siglaDRE, string codigoDRE, string tipoEscola, string siglaTipoEscola, int codigoTipoEscola)
        {
            CodigoEscola = codigoEscola;
            NomeEscola = nomeEscola;
            NomeDRE = nomeDRE;
            SiglaDRE = siglaDRE;
            CodigoDRE = codigoDRE;
            TipoEscola = tipoEscola;
            SiglaTipoEscola = siglaTipoEscola;
            CodigoTipoEscola = codigoTipoEscola;
        }

        public string? CodigoEscola { get; set; }
        public string? NomeEscola { get; set; }
        public string? NomeDRE { get; set; }
        public string? SiglaDRE { get; set; }
        public string? CodigoDRE { get; set; }
        public string? TipoEscola { get; set; }
        public string? SiglaTipoEscola { get; set; }
        public int? CodigoTipoEscola { get; set; }

    }
}
