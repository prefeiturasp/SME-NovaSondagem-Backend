using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Questionario;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

[ExcludeFromCodeCoverage]
public class QuestionarioBimestreMap : IEntityTypeConfiguration<QuestionarioBimestre>
{
    public void Configure(EntityTypeBuilder<QuestionarioBimestre> builder)
    {
        builder.ToTable("questionario_bimestre");

        builder.HasKey(x => x.Id).HasName("pk_questionario_bimestre");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.QuestionarioId)
            .HasColumnName("questionario_id")
            .IsRequired();

        builder.Property(x => x.BimestreId)
            .HasColumnName("bimestre_id")
            .IsRequired();

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        ConfigurarAuditoria(builder);

        builder.HasOne(qb => qb.Questionario)
            .WithMany(q => q.QuestionariosBimestres)
            .HasForeignKey(qb => qb.QuestionarioId)
            .HasConstraintName("fk_questionario_bimestre_questionario")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(qb => qb.Bimestre)
            .WithMany(b => b.QuestionariosBimestres)
            .HasForeignKey(qb => qb.BimestreId)
            .HasConstraintName("fk_questionario_bimestre_bimestre")
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(x => x.QuestionarioId)
            .HasDatabaseName("ix_questionario_bimestre_questionario_id");

        builder.HasIndex(x => x.BimestreId)
            .HasDatabaseName("ix_questionario_bimestre_bimestre_id");

        builder.HasIndex(x => new { x.QuestionarioId, x.BimestreId })
            .HasDatabaseName("uk_questionario_bimestre_questionario_bimestre")
            .IsUnique()
            .HasFilter("excluido = false");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<QuestionarioBimestre> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}