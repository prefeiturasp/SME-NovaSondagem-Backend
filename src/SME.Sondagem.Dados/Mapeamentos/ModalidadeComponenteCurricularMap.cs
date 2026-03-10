using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

[ExcludeFromCodeCoverage]
public class ModalidadeComponenteCurricularMap : IEntityTypeConfiguration<ModalidadeComponenteCurricular>
{
    public void Configure(EntityTypeBuilder<ModalidadeComponenteCurricular> builder)
    {
        builder.ToTable("modalidade_componente_curricular");

        builder.HasKey(x => x.Id).HasName("pk_modalidade_componente_curricular");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.ModalidadeId)
            .HasColumnName("modalidade_id")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ComponenteCurricularId)
            .HasColumnName("componente_curricular_id")
            .IsRequired();

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        builder.HasIndex(x => new { x.ModalidadeId, x.ComponenteCurricularId })
            .HasDatabaseName("uk_modalidade_componente")
            .IsUnique();

        builder.HasOne(x => x.ComponenteCurricular)
            .WithMany(x => x.ModalidadeComponenteCurricular)
            .HasForeignKey(x => x.ComponenteCurricularId)
            .HasConstraintName("fk_modalidade_componente_curricular_componente")
            .OnDelete(DeleteBehavior.Cascade);

        ConfigurarAuditoria(builder);
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<ModalidadeComponenteCurricular> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}