using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Questionario;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

public class QuestionarioMap : IEntityTypeConfiguration<Questionario>
{
    [ExcludeFromCodeCoverage]
    public void Configure(EntityTypeBuilder<Questionario> builder)
    {
        builder.ToTable("questionario");

        builder.HasKey(x => x.Id).HasName("pk_questionario");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .HasConversion<int>();

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        builder.Property(x => x.AnoLetivo)
            .HasColumnName("ano_letivo")
            .IsRequired();

        builder.Property(x => x.ModalidadeId)
            .HasColumnName("modalidade_id");

        builder.Property(x => x.SerieAno)
            .HasColumnName("serie_ano");

        builder.Property(x => x.ComponenteCurricularId)
            .HasColumnName("componente_curricular_id")
            .IsRequired();

        builder.Property(x => x.ProficienciaId)
            .HasColumnName("proficiencia_id")
            .IsRequired();

        ConfigurarAuditoria(builder);

        builder.HasOne(x => x.ComponenteCurricular)
            .WithMany(x => x.Questionarios)
            .HasForeignKey(x => x.ComponenteCurricularId)
            .HasConstraintName("fk_questionario_componente");

        builder.HasOne(x => x.Proficiencia)
            .WithMany(x => x.Questionarios)
            .HasForeignKey(x => x.ProficienciaId)
            .HasConstraintName("fk_questionario_proficiencia");

        builder.HasMany(x => x.Questoes)
            .WithOne(x => x.Questionario)
            .HasForeignKey(x => x.QuestionarioId)
            .HasConstraintName("fk_questao_questionario")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Sondagens)
            .WithOne(x => x.Questionario)
            .HasForeignKey(x => x.QuestionarioId)
            .HasConstraintName("fk_sondagem_questionario");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<Questionario> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}