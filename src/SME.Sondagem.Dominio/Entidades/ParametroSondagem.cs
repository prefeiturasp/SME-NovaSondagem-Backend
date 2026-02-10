using SME.Sondagem.Dominio.Enums;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dominio.Entidades
{
    [ExcludeFromCodeCoverage]
    public class ParametroSondagem : EntidadeBase
    {
        public bool Ativo { get; set; }
        public string? Descricao { get; set; }
        public string? Nome { get; set; }
        public TipoParametroSondagem Tipo { get; set; }

        public void Atualizar(bool ativo, string? descricao, string? nome, TipoParametroSondagem tipo)
        {
            Ativo = ativo;
            Descricao = descricao;
            Nome = nome;
            Tipo = tipo;
        }
    }
}
