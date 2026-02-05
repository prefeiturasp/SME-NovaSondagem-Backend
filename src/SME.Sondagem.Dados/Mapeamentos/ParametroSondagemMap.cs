using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

[ExcludeFromCodeCoverage]
public class ParametroSondagemMap : IEntityTypeConfiguration<ParametroSondagem>
{
    public void Configure(EntityTypeBuilder<ParametroSondagem> builder)
    {
        builder.ToTable("parametro_sondagem");

        builder.HasKey(x => x.Id).HasName("pk_parametro_sondagem");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.Ativo)
            .HasColumnName("ativo")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(500);

        builder.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        ConfigurarAuditoria(builder);

        // Índices
        builder.HasIndex(x => x.Tipo)
            .HasDatabaseName("ix_parametro_sondagem_tipo");

        builder.HasIndex(x => x.Ativo)
            .HasDatabaseName("ix_parametro_sondagem_ativo");

        builder.HasIndex(x => new { x.Tipo, x.Ativo })
            .HasDatabaseName("ix_parametro_sondagem_tipo_ativo")
            .HasFilter("excluido = false");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<ParametroSondagem> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}