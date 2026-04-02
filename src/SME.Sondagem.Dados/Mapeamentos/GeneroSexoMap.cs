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
        }
    }
}
