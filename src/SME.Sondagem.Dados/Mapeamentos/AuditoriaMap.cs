using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Mapeamentos;

public class AuditoriaMap : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        builder.ToTable("auditoria");

        builder.HasKey(x => x.Id).HasName("pk_auditoria");
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Acao)
            .HasColumnName("acao")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Chave)
            .HasColumnName("chave")
            .IsRequired();

        builder.Property(x => x.Data)
            .HasColumnName("data")
            .IsRequired();

        builder.Property(x => x.Entidade)
            .HasColumnName("entidade")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.RF)
            .HasColumnName("rf")
            .HasMaxLength(50);

        builder.Property(x => x.Usuario)
            .HasColumnName("usuario")
            .HasMaxLength(200);

        builder.Property(x => x.Perfil)
            .HasColumnName("perfil");

        builder.Property(x => x.Administrador)
            .HasColumnName("administrador")
            .HasMaxLength(200);

        // Índices
        builder.HasIndex(x => x.Entidade)
            .HasDatabaseName("ix_auditoria_entidade");

        builder.HasIndex(x => x.Chave)
            .HasDatabaseName("ix_auditoria_chave");

        builder.HasIndex(x => x.Data)
            .HasDatabaseName("ix_auditoria_data");

        builder.HasIndex(x => new { x.Entidade, x.Chave })
            .HasDatabaseName("ix_auditoria_entidade_chave");

        // Relacionamento com Detalhes
        builder.HasMany(a => a.Detalhes)
            .WithOne(d => d.Auditoria)
            .HasForeignKey(d => d.AuditoriaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}