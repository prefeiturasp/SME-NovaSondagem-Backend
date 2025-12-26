using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Mapeamentos;

public class OpcaoRespostaMap : IEntityTypeConfiguration<OpcaoResposta>
{
    public void Configure(EntityTypeBuilder<OpcaoResposta> builder)
    {
        builder.ToTable("opcao_resposta");

        builder.HasKey(x => x.Id).HasName("pk_opcao_resposta");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.DescricaoOpcaoResposta)
            .HasColumnName("descricao_opcao_resposta")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Legenda)
            .HasColumnName("legenda")
            .HasColumnType("text");

        builder.Property(x => x.CorFundo)
            .HasColumnName("cor_fundo")
            .HasColumnType("text");

        builder.Property(x => x.CorTexto)
            .HasColumnName("cor_texto")
            .HasColumnType("text");

        builder.Property(x => x.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false);

        ConfigurarAuditoria(builder);

        builder.HasIndex(x => x.DescricaoOpcaoResposta)
            .HasDatabaseName("uk_opcao_resposta_desc")
            .IsUnique();

        builder.HasMany(x => x.QuestaoOpcoes)
            .WithOne(x => x.OpcaoResposta)
            .HasForeignKey(x => x.OpcaoRespostaId)
            .HasConstraintName("fk_questao_opcao_opcao");

        builder.HasMany(x => x.Respostas)
            .WithOne(x => x.OpcaoResposta)
            .HasForeignKey(x => x.OpcaoRespostaId)
            .HasConstraintName("fk_resposta_opcao");
    }

    private static void ConfigurarAuditoria(EntityTypeBuilder<OpcaoResposta> builder)
    {
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
        builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
        builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
    }
}