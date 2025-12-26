using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Ciclo;

namespace SME.Sondagem.Dados.Mapeamentos;

public class CicloMap : IEntityTypeConfiguration<Ciclo>
{
    public void Configure(EntityTypeBuilder<Ciclo> builder)
    {
        builder.ToTable("ciclo");

        builder.HasKey(x => x.Id).HasName("pk_ciclo");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.CodCicloEnsinoEol)
            .HasColumnName("cod_ciclo_ensino_eol")
            .IsRequired();

        builder.Property(x => x.DescCiclo)
            .HasColumnName("desc_ciclo")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        ConfigurarAuditoria(builder);

        builder.HasIndex(x => x.CodCicloEnsinoEol)
            .HasDatabaseName("uk_ciclo_cod_eol")
            .IsUnique();

        builder.HasIndex(x => x.DescCiclo)
            .HasDatabaseName("uk_ciclo_desc")
            .IsUnique();

        builder.HasMany(x => x.Questionarios)
            .WithOne(x => x.Ciclo)
            .HasForeignKey(x => x.CicloId)
            .HasConstraintName("fk_questionario_ciclo");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<Ciclo> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}