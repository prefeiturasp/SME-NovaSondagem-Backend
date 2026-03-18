using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class ControleAcessoOptionsMap : IEntityTypeConfiguration<ControleAcessoOptions>
    {
        public void Configure(EntityTypeBuilder<ControleAcessoOptions> builder)
        {

            builder.ToTable("controle_acesso_options");

            builder.Property(e => e.GrupoSituacao)
                .IsRequired();

            builder.Property(e => e.SistemaId)
                .IsRequired();

            builder.Property(e => e.ModuloId)
                .IsRequired();

            builder.Property(e => e.CacheDuracaoMinutos)
                .IsRequired();

            builder.HasMany(e => e.ConfiguracaoPerfis)
                .WithOne()
                .HasForeignKey("ControleAcessoOptionsId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
