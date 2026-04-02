namespace SME.Sondagem.Dominio.Entidades
{
    public class GeneroSexo : EntidadeBase
    {
        public required string Descricao { get; set; }
        public string? Sigla { get; set; }
    }
}
