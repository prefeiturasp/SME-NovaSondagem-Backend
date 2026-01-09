using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dados.Mapeamentos
{
    public class SondagemPeriodoBimestreMap : IEntityTypeConfiguration<SondagemPeriodoBimestre>
    {
        public void Configure(EntityTypeBuilder<SondagemPeriodoBimestre> builder)
        {
            builder.ToTable("sondagem_periodo_bimestre");

            builder.HasKey(x => x.Id).HasName("pk_sondagem_periodo_bimestre");
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.Excluido)
                .HasColumnName("excluido")
                .HasDefaultValue(false);

            ConfigurarAuditoria(builder);

            builder.Property(x => x.SondagemId)
                .HasColumnName("sondagem_id")
                .IsRequired();

            builder.Property(x => x.BimestreId)
                .HasColumnName("bimestre_id")
                .IsRequired();

            builder.Property(x => x.DataInicio).HasColumnName("data_inicio");
            builder.Property(x => x.DataFim).HasColumnName("data_fim");

            builder.HasOne(x => x.Sondagem)
                .WithMany(x => x.PeriodosBimestre)
                .HasForeignKey(x => x.SondagemId)
                .HasConstraintName("fk_sondagem_sondagem_periodo_bimestre");

            builder.HasOne(x => x.Bimestre)
                .WithMany(x => x.PeriodosBimestre)
                .HasForeignKey(x => x.BimestreId)
                .HasConstraintName("fk_bimestre_sondagem_periodo_bimestre");
        }

        private static void ConfigurarAuditoria(EntityTypeBuilder<SondagemPeriodoBimestre> builder)
        {
            builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
            builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
            builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
        }
    }
}
