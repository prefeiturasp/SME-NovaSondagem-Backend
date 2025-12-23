using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.ComponenteCurricular;

namespace SME.Sondagem.Dados.Mapeamentos
{
    public class ComponenteCurricularMap : IEntityTypeConfiguration<ComponenteCurricular>
    {
        public void Configure(EntityTypeBuilder<ComponenteCurricular> builder)
        {
            builder.ToTable("componente_curricular");

            builder.HasKey(x => x.Id).HasName("pk_componente_curricular");
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.Nome)
                .HasColumnName("nome")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Ano)
                .HasColumnName("ano");

            builder.Property(x => x.Modalidade)
                .HasColumnName("modalidade")
                .HasMaxLength(100);

            builder.Property(x => x.CodigoEol)
                .HasColumnName("codigo_eol")
                .IsRequired();

            ConfigurarAuditoria(builder);

            builder.HasIndex(x => new { x.Nome, x.Ano, x.Modalidade })
                .HasDatabaseName("uk_componente_nome_ano_modalidade")
                .IsUnique();

            // Relacionamentos
            builder.HasMany(x => x.Proficiencias)
                .WithOne(x => x.ComponenteCurricular)
                .HasForeignKey(x => x.ComponenteCurricularId)
                .HasConstraintName("fk_proficiencia_componente");

            builder.HasMany(x => x.Questionarios)
                .WithOne(x => x.ComponenteCurricular)
                .HasForeignKey(x => x.ComponenteCurricularId)
                .HasConstraintName("fk_questionario_componente");
        }

        private static void ConfigurarAuditoria(EntityTypeBuilder<ComponenteCurricular> builder)
        {
            builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
            builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
            builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
        }
    }
}