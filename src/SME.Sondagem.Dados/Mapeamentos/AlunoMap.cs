using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

[ExcludeFromCodeCoverage]
public class AlunoMap : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder.ToTable("aluno");

        builder.HasKey(x => x.Id).HasName("pk_aluno");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.RaAluno)
            .HasColumnName("ra_aluno")
            .HasMaxLength(20);

        builder.Property(x => x.NomeAluno)
            .HasColumnName("nome_aluno")
            .HasMaxLength(200);

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        builder.Property(x => x.IsPap)
            .HasColumnName("is_pap")
            .HasDefaultValue(false);

        builder.Property(x => x.IsAee)
            .HasColumnName("is_aee")
            .HasDefaultValue(false);

        builder.Property(x => x.IsPcd)
            .HasColumnName("is_pcd")
            .HasDefaultValue(false);

        builder.Property(x => x.DeficienciaId)
            .HasColumnName("deficiencia_id");

        builder.Property(x => x.DeficienciaNome)
            .HasColumnName("deficiencia_nome")
            .HasMaxLength(100);

        builder.Property(x => x.RacaId)
            .HasColumnName("raca_id");

        builder.Property(x => x.RacaNome)
            .HasColumnName("raca_nome")
            .HasMaxLength(50);

        builder.Property(x => x.CorId)
            .HasColumnName("cor_id");

        builder.Property(x => x.CorNome)
            .HasColumnName("cor_nome")
            .HasMaxLength(50);

        ConfigurarAuditoria(builder);

        builder.HasIndex(x => x.RaAluno)
            .HasDatabaseName("uk_aluno_ra")
            .IsUnique()
            .HasFilter("ra_aluno IS NOT NULL");

        builder.HasMany(x => x.Respostas)
            .WithOne(x => x.Aluno)
            .HasForeignKey(x => x.AlunoId)
            .HasConstraintName("fk_resposta_aluno");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<Aluno> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}