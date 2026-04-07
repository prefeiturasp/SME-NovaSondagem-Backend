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

            builder.Property(e => e.GrupoSituacao).HasColumnName("grupo_situacao")
                .IsRequired();

            builder.Property(e => e.SistemaId).HasColumnName("sistema_id")
                .IsRequired();

            builder.Property(e => e.ModuloId).HasColumnName("modulo_id")
                .IsRequired();

            builder.Property(e => e.CacheDuracaoMinutos).HasColumnName("cache_duracao_minutos")
                .IsRequired();

            builder.HasMany(e => e.ConfiguracaoPerfis)
                .WithOne()
                .HasForeignKey("ControleAcessoOptionsId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
