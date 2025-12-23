using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Mapeamentos;

public class QuestaoOpcaoRespostaMap : IEntityTypeConfiguration<QuestaoOpcaoResposta>
{
    public void Configure(EntityTypeBuilder<QuestaoOpcaoResposta> builder)
    {
        builder.ToTable("questao_opcao_resposta");

        builder.HasKey(x => x.Id).HasName("pk_questao_opcao_resposta");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.QuestaoId)
            .HasColumnName("questao_id")
            .IsRequired();

        builder.Property(x => x.OpcaoRespostaId)
            .HasColumnName("opcao_resposta_id")
            .IsRequired();

        builder.Property(x => x.Ordem)
            .HasColumnName("ordem")
            .IsRequired();

        ConfigurarAuditoria(builder);

        builder.HasIndex(x => new { x.QuestaoId, x.OpcaoRespostaId })
            .HasDatabaseName("uk_questao_opcao")
            .IsUnique();

        builder.HasOne(x => x.Questao)
            .WithMany(x => x.QuestaoOpcoes)
            .HasForeignKey(x => x.QuestaoId)
            .HasConstraintName("fk_questao_opcao_questao")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OpcaoResposta)
            .WithMany(x => x.QuestaoOpcoes)
            .HasForeignKey(x => x.OpcaoRespostaId)
            .HasConstraintName("fk_questao_opcao_opcao");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<QuestaoOpcaoResposta> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}