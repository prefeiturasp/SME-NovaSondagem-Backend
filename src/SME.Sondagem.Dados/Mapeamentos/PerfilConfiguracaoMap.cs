using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Configuration;

namespace SME.Sondagem.Dados.Mapeamentos
{
    public class PerfilConfiguracaoMap : IEntityTypeConfiguration<PerfilConfiguracao>
    {
        public void Configure(EntityTypeBuilder<PerfilConfiguracao> builder)
        {

            builder.ToTable("perfil_configuracao");

            builder.Property(e => e.Nome).HasColumnName("nome")
                .HasMaxLength(200);

            builder.Property(e => e.Codigo).HasColumnName("codigo")
                   .HasMaxLength(200);

            builder.Property(e => e.TipoValidacao).HasColumnName("tipo_validacao")
                .HasMaxLength(50)
                .HasComment("Valores esperados: Regencia, UE, AcessoTotal");

            builder.Property(e => e.ConsultarAbrangencia).HasColumnName("consultar_abrangencia")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.AcessoIrrestrito).HasColumnName("acesso_irrestrito")
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
