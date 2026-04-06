using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Mapeamentos
{
    public class ProgramaAtendimentoMap : IEntityTypeConfiguration<ProgramaAtendimento>
    {
        public void Configure(EntityTypeBuilder<ProgramaAtendimento> builder)
        {
            builder.ToTable("programa_atendimento");
            builder.Property(x => x.Descricao)
                    .HasColumnName("descricao").IsRequired()
                    .HasMaxLength(20);

            builder.HasIndex(x => x.Descricao)
                   .IsUnique()
                   .HasDatabaseName("IX_ProgramaAtendimento_Descricao");


            builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(x => x.Excluido).HasColumnName("excluido").IsRequired();
            builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
            builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
            builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
            builder.Property(x => x.Id).HasColumnName("id");
        }
    }
}
