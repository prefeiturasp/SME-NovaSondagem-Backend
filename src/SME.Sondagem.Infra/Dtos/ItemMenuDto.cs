namespace SME.Sondagem.Infrastructure.Dtos
{
    public class ItemMenuDto(int id, string descricao)
    {
        public int Id { get; set; } = id;
        public string Descricao { get; set; } = descricao;
    }
}
