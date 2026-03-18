using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Mapeamentos
{
    public class PerfilInfoMap : IEntityTypeConfiguration<PerfilInfo>
    {
        public void Configure(EntityTypeBuilder<PerfilInfo> builder)
        {

            builder.ToTable("perfil_info");

            builder.Property(e => e.Codigo)
                .IsRequired();

            builder.HasIndex(e => e.Codigo)
                .IsUnique();

            builder.Property(e => e.Nome)
                .HasMaxLength(200);

            builder.Property(e => e.PermiteConsultar)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.PermiteInserir)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.PermiteAlterar)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.PermiteExcluir)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.TipoValidacao)
                .HasMaxLength(50);

            builder.Property(e => e.ConsultarAbrangencia)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.AcessoIrrestrito)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
