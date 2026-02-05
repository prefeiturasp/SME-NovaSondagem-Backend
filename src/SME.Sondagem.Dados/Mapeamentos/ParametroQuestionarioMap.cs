using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

[ExcludeFromCodeCoverage]
public class ParametroQuestionarioMap : IEntityTypeConfiguration<ParametroQuestionario>
{
    public void Configure(EntityTypeBuilder<ParametroQuestionario> builder)
    {
        builder.ToTable("parametro_questionario");

        builder.HasKey(x => x.Id).HasName("pk_parametro_questionario");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.IdQuestionario)
            .HasColumnName("id_questionario")
            .IsRequired();

        builder.Property(x => x.IdParametroSondagem)
            .HasColumnName("id_parametro_sondagem")
            .IsRequired();

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .IsRequired()
            .HasDefaultValue(false);

        ConfigurarAuditoria(builder);

        builder.HasOne(pq => pq.Questionario)
            .WithMany(q => q.ParametrosQuestionario)
            .HasForeignKey(pq => pq.IdQuestionario)
            .HasConstraintName("fk_parametro_questionario_questionario_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pq => pq.ParametroSondagem)
            .WithMany()
            .HasForeignKey(pq => pq.IdParametroSondagem)
            .HasConstraintName("fk_parametro_questionario_parametro_sondagem_id")
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(x => x.IdQuestionario)
            .HasDatabaseName("ix_parametro_questionario_questionario_id");

        builder.HasIndex(x => x.IdParametroSondagem)
            .HasDatabaseName("ix_parametro_questionario_parametro_sondagem_id");

        builder.HasIndex(x => new { x.IdQuestionario, x.IdParametroSondagem })
            .HasDatabaseName("uk_parametro_questionario_questionario_parametro_sondagem")
            .IsUnique()
            .HasFilter("excluido = false");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<ParametroQuestionario> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}