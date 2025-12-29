using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Mapeamentos;

public class AuditoriaDetalheMap : IEntityTypeConfiguration<AuditoriaDetalhe>
{
    public void Configure(EntityTypeBuilder<AuditoriaDetalhe> builder)
    {
        builder.ToTable("auditoria_detalhe");

        builder.HasKey(x => x.Id).HasName("pk_auditoria_detalhe");
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.AuditoriaId)
            .HasColumnName("auditoria_id")
            .IsRequired();

        builder.Property(x => x.NomePropriedade)
            .HasColumnName("nome_propriedade")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ValorAntigo)
            .HasColumnName("valor_antigo")
            .HasColumnType("text");

        builder.Property(x => x.ValorNovo)
            .HasColumnName("valor_novo")
            .HasColumnType("text");

        builder.HasOne(x => x.Auditoria)
            .WithMany(a => a.Detalhes)
            .HasForeignKey(x => x.AuditoriaId)
            .HasConstraintName("fk_auditoria_detalhe_auditoria")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.AuditoriaId)
            .HasDatabaseName("ix_auditoria_detalhe_auditoria_id");

        builder.HasIndex(x => x.NomePropriedade)
            .HasDatabaseName("ix_auditoria_detalhe_nome_propriedade");
    }
}