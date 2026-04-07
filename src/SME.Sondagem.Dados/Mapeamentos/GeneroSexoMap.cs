using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class GeneroSexoMap : IEntityTypeConfiguration<GeneroSexo>
    {
        public void Configure(EntityTypeBuilder<GeneroSexo> builder)
        {
            builder.ToTable("genero_sexo");
            builder.Property(x => x.Descricao)
                    .HasColumnName("descricao").IsRequired()
                    .HasMaxLength(50);

            builder.Property(x => x.Sigla)
                    .HasColumnName("sigla").IsRequired()
                    .HasMaxLength(10);


            builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(x => x.Excluido).HasColumnName("excluido").IsRequired();
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AlteradoEm).HasColumnName("alterado_em");
            builder.Property(x => x.AlteradoPor).HasColumnName("alterado_por").HasMaxLength(200);
            builder.Property(x => x.CriadoRF).HasColumnName("criado_rf").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AlteradoRF).HasColumnName("alterado_rf").HasMaxLength(200);
        }
    }
}
