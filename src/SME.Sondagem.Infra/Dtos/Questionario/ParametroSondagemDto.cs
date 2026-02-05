using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public class ParametroSondagemDto : BaseDto
    {
        public bool Ativo { get; set; }
        public string? Descricao { get; set; }
        public string? Nome { get; set; }
        public TipoParametroSondagem Tipo { get; set; }
    }
}
