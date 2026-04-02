using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class RacaCorMap : IEntityTypeConfiguration<RacaCor>
    {
        public void Configure(EntityTypeBuilder<RacaCor> builder)
        {
            builder.ToTable("raca_cor");

            builder.Property(x => x.Descricao)
                                .HasColumnName("descricao").IsRequired()
                                .HasMaxLength(20);

            builder.Property(x => x.CodigoEolRacaCor).IsRequired()
                    .HasColumnName("codigo_eol_racacor");


            builder.HasIndex(x => x.CodigoEolRacaCor)
               .IsUnique()
               .HasDatabaseName("IX_RacaCor_CodigoEol");

            builder.HasIndex(x => x.Descricao)
                   .IsUnique()
                   .HasDatabaseName("IX_RacaCor_Descricao");
        }
    }
}
