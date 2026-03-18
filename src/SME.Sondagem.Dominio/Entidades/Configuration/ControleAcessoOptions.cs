namespace SME.Sondagem.Dominio.Entidades.Configuration
{
    public class ControleAcessoOptions : EntidadeBase
    {
        public int GrupoSituacao { get; set; }
        public int SistemaId { get; set; }
        public int ModuloId { get; set; }

        public int CacheDuracaoMinutos { get; set; }

        public Dictionary<string, PerfilConfiguracao> ConfiguracaoPerfis { get; set; }
    }
}
