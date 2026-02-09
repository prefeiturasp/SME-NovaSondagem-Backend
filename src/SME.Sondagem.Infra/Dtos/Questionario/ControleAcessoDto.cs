using SME.Sondagem.Dominio.Enums;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    [ExcludeFromCodeCoverage]
    public class ControleAcessoDto
    {
        public bool Regencia { get; set; }
        public IEnumerable<string> TurmaCodigos { get; set; } = [];
        public IEnumerable<string> IdUes { get; set; } = [];
    }
}
