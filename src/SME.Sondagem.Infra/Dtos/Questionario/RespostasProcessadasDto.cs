using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public class RespostasProcessadasDto
    {
        public string? InseridoPor { get; set; }
        public string? AlteradoPor { get; set; }
        public required Dictionary<(int CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> RespostasConvertidas { get; set; }
    }
}
