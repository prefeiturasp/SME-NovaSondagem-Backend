using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

[ExcludeFromCodeCoverage]
public class BimestreMap : IEntityTypeConfiguration<Bimestre>
{
    public void Configure(EntityTypeBuilder<Bimestre> builder)
    {
        builder.ToTable("bimestre");

        builder.HasKey(x => x.Id).HasName("pk_bimestre");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.CodBimestreEnsinoEol)
            .HasColumnName("cod_bimestre_ensino_eol")
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        ConfigurarAuditoria(builder);

        builder.HasIndex(x => x.CodBimestreEnsinoEol)
            .HasDatabaseName("uk_bimestre_cod_eol")
            .IsUnique();

        builder.HasIndex(x => x.Descricao)
            .HasDatabaseName("uk_bimestre_desc")
            .IsUnique();
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<Bimestre> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}