using SME.Sondagem.Dominio.Enums;
using System.Text.Json.Serialization;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public class ParametroSondagemDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
        public int Id { get; set; }
        public bool Ativo { get; set; }
        public string? Descricao { get; set; }
        public string? Nome { get; set; }
        public TipoParametroSondagem Tipo { get; set; }
    }
}
