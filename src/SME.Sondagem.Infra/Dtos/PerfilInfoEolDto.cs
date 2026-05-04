namespace SME.Sondagem.Infrastructure.Dtos
{
    public class PerfilInfoEolDto
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public bool PermiteConsultar { get; set; }
        public bool PermiteInserir { get; set; }
        public bool PermiteAlterar { get; set; }
        public bool PermiteExcluir { get; set; }
    }
}
