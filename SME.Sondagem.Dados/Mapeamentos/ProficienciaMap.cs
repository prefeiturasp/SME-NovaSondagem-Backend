using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SME.Sondagem.Dominio.Entidades.Questionario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Dados.Mapeamentos
{
    public class ProficienciaMap : IEntityTypeConfiguration<Proficiencia>
    {
        public void Configure(EntityTypeBuilder<Proficiencia> builder)
        {
            builder.ToTable("proficiencia");

            builder.HasKey(x => x.Id).HasName("pk_proficiencia");
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.Nome)
                .HasColumnName("nome")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ComponenteCurricularId)
                .HasColumnName("componente_curricular_id")
                .IsRequired();

            ConfigurarAuditoria(builder);

            builder.HasIndex(x => new { x.Nome, x.ComponenteCurricularId })
                .HasDatabaseName("uk_proficiencia_nome_componente")
                .IsUnique();

            builder.HasOne(x => x.ComponenteCurricular)
                .WithMany(x => x.Proficiencias)
                .HasForeignKey(x => x.ComponenteCurricularId)
                .HasConstraintName("fk_proficiencia_componente");

            builder.HasMany(x => x.Questionarios)
                .WithOne(x => x.Proficiencia)
                .HasForeignKey(x => x.ProficienciaId)
                .HasConstraintName("fk_questionario_proficiencia");
        }

        private static void ConfigurarAuditoria(EntityTypeBuilder<Proficiencia> builder)
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