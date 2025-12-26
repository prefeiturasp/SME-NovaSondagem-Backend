using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Questionario;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

public class GrupoQuestoesMap : IEntityTypeConfiguration<GrupoQuestoes>
{
    [ExcludeFromCodeCoverage]
    public void Configure(EntityTypeBuilder<GrupoQuestoes> builder)
    {
        builder.ToTable("grupo_questoes");

        builder.HasKey(x => x.Id).HasName("pk_grupo_questoes");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.Titulo)
            .HasColumnName("titulo")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Subtitulo)
            .HasColumnName("subtitulo")
            .HasColumnType("text");

        ConfigurarAuditoria(builder);

        builder.HasMany(x => x.Questoes)
            .WithOne(x => x.GrupoQuestoes)
            .HasForeignKey(x => x.GrupoQuestoesId)
            .HasConstraintName("fk_questao_grupo")
            .IsRequired(false);
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<GrupoQuestoes> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}