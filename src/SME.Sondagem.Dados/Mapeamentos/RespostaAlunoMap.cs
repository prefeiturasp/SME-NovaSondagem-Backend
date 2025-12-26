using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

[ExcludeFromCodeCoverage]
public class RespostaAlunoMap : IEntityTypeConfiguration<RespostaAluno>
{
    public void Configure(EntityTypeBuilder<RespostaAluno> builder)
    {
        builder.ToTable("resposta_aluno");

        builder.HasKey(x => x.Id).HasName("pk_resposta_aluno");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.SondagemId)
            .HasColumnName("sondagem_id")
            .IsRequired();

        builder.Property(x => x.AlunoId)
            .HasColumnName("aluno_id")
            .IsRequired();

        builder.Property(x => x.QuestaoId)
            .HasColumnName("questao_id")
            .IsRequired();

        builder.Property(x => x.OpcaoRespostaId)
            .HasColumnName("opcao_resposta_id")
            .IsRequired();

        builder.Property(x => x.DataResposta)
            .HasColumnName("data_resposta")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        ConfigurarAuditoria(builder);

        builder.HasIndex(x => new { x.SondagemId, x.AlunoId, x.QuestaoId })
            .HasDatabaseName("uk_resposta_sondagem_aluno_questao")
            .IsUnique();

        builder.HasOne(x => x.Sondagem)
            .WithMany(x => x.Respostas)
            .HasForeignKey(x => x.SondagemId)
            .HasConstraintName("fk_resposta_sondagem")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Aluno)
            .WithMany(x => x.Respostas)
            .HasForeignKey(x => x.AlunoId)
            .HasConstraintName("fk_resposta_aluno");

        builder.HasOne(x => x.Questao)
            .WithMany(x => x.Respostas)
            .HasForeignKey(x => x.QuestaoId)
            .HasConstraintName("fk_resposta_questao");

        builder.HasOne(x => x.OpcaoResposta)
            .WithMany(x => x.Respostas)
            .HasForeignKey(x => x.OpcaoRespostaId)
            .HasConstraintName("fk_resposta_opcao");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<RespostaAluno> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}