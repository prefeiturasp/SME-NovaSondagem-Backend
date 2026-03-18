namespace SME.Sondagem.Dominio.Entidades.Configuration
{
    public class PerfilConfiguracao : EntidadeBase
    {
        public string? Nome { get; set; }
        public string? TipoValidacao { get; set; } // "Regencia", "UE", "AcessoTotal"
        public bool ConsultarAbrangencia { get; set; }
        public bool AcessoIrrestrito { get; set; }
    }
}
