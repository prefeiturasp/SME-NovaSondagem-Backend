using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Mapeamentos;

public class QuestaoMap : IEntityTypeConfiguration<Questao>
{
    public void Configure(EntityTypeBuilder<Questao> builder)
    {
        builder.ToTable("questao");

        builder.HasKey(x => x.Id).HasName("pk_questao");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.QuestionarioId)
            .HasColumnName("questionario_id")
            .IsRequired();

        builder.Property(x => x.GrupoQuestoesId)
            .HasColumnName("grupo_questoes_id");

        builder.Property(x => x.Ordem)
            .HasColumnName("ordem")
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Observacao)
            .HasColumnName("observacao")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Obrigatorio)
            .HasColumnName("obrigatorio")
            .HasDefaultValue(false);

        builder.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .HasConversion<int>();

        builder.Property(x => x.Opcionais)
            .HasColumnName("opcionais")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.SomenteLeitura)
            .HasColumnName("somente_leitura")
            .HasDefaultValue(false);

        builder.Property(x => x.Dimensao)
            .HasColumnName("dimensao");

        builder.Property(x => x.Tamanho)
            .HasColumnName("tamanho");

        builder.Property(x => x.Mascara)
            .HasColumnName("mascara")
            .HasMaxLength(50);

        builder.Property(x => x.PlaceHolder)
            .HasColumnName("place_holder")
            .HasMaxLength(200);

        builder.Property(x => x.NomeComponente)
            .HasColumnName("nome_componente")
            .HasMaxLength(200);

        ConfigurarAuditoria(builder);

        // Relacionamentos
        builder.HasOne(x => x.Questionario)
            .WithMany(x => x.Questoes)
            .HasForeignKey(x => x.QuestionarioId)
            .HasConstraintName("fk_questao_questionario")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.GrupoQuestoes)
            .WithMany(x => x.Questoes)
            .HasForeignKey(x => x.GrupoQuestoesId)
            .HasConstraintName("fk_questao_grupo")
            .IsRequired(false);

        builder.HasMany(x => x.QuestaoOpcoes)
            .WithOne(x => x.Questao)
            .HasForeignKey(x => x.QuestaoId)
            .HasConstraintName("fk_questao_opcao_questao")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Respostas)
            .WithOne(x => x.Questao)
            .HasForeignKey(x => x.QuestaoId)
            .HasConstraintName("fk_resposta_questao");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<Questao> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}