namespace SME.Sondagem.Infrastructure.Dtos
{
    public class PerfilInfoSondagemDto
    {
        public Guid Codigo { get; set; }
        public string? Nome { get; set; }

        public bool PermiteConsultar { get; set; }
        public bool PermiteInserir { get; set; }
        public bool PermiteAlterar { get; set; }
        public bool PermiteExcluir { get; set; }

        public string? TipoValidacao { get; set; }
        public bool ConsultarAbrangencia { get; set; }
        public bool AcessoIrrestrito { get; set; }
    }
}
