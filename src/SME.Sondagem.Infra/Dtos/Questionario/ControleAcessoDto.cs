using SME.Sondagem.Dominio.Enums;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    [ExcludeFromCodeCoverage]
    public class ControleAcessoDto
    {
        public int Codigo { get; set; }
        public int CodigoComponenteTerritorioSaber { get; set; }
        public int? CodigoComponenteCurricularPai { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public bool Regencia { get; set; }
        public bool PlanejamentoRegencia { get; set; }
        public bool TerritorioSaber { get; set; }
        public string? TurmaCodigo { get; set; }
        public bool ExibirComponenteEol { get; set; }
        public string? Professor { get; set; }
        public IEnumerable<int>? CodigosTerritoriosAgrupamento { get; set; }
    }
}
