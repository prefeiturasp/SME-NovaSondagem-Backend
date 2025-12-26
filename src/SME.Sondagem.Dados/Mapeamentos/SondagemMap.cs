using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

[ExcludeFromCodeCoverage]
public class SondagemMap : IEntityTypeConfiguration<Dominio.Entidades.Sondagem.Sondagem>
{
    public void Configure(EntityTypeBuilder<Dominio.Entidades.Sondagem.Sondagem> builder)
    {
        builder.ToTable("sondagem");

        builder.HasKey(x => x.Id).HasName("pk_sondagem");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.QuestionarioId)
            .HasColumnName("questionario_id")
            .IsRequired();

        builder.Property(x => x.DataAplicacao)
            .HasColumnName("data_aplicacao")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        ConfigurarAuditoria(builder);

        builder.HasOne(x => x.Questionario)
            .WithMany(x => x.Sondagens)
            .HasForeignKey(x => x.QuestionarioId)
            .HasConstraintName("fk_sondagem_questionario");

        builder.HasMany(x => x.Respostas)
            .WithOne(x => x.Sondagem)
            .HasForeignKey(x => x.SondagemId)
            .HasConstraintName("fk_resposta_sondagem")
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<Dominio.Entidades.Sondagem.Sondagem> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}