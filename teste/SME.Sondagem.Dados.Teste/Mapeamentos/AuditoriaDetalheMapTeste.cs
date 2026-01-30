using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SME.Sondagem.Dados.Mapeamentos;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class AuditoriaDetalheMapTeste
    {
        private class TestDbContext : DbContext
        {
            public DbSet<AuditoriaDetalhe> AuditoriaDetalhes { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseInMemoryDatabase("AuditoriaDetalheMapTest");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.ApplyConfiguration(new AuditoriaDetalheMap());
        }

        [Fact]
        public void Deve_Mapear_Propriedades_E_Indices_Corretamente()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(AuditoriaDetalhe));

            Assert.NotNull(entity);

            var primaryKey = entity.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Equal("pk_auditoria_detalhe", primaryKey.GetName());

            var auditoriaIdProperty = entity.FindProperty("AuditoriaId");
            Assert.NotNull(auditoriaIdProperty);
            Assert.Equal("auditoria_id", auditoriaIdProperty.GetColumnName(StoreObjectIdentifier.Table("auditoria_detalhe", null)));

            var nomePropriedadeProperty = entity.FindProperty("NomePropriedade");
            Assert.NotNull(nomePropriedadeProperty);
            Assert.Equal("nome_propriedade", nomePropriedadeProperty.GetColumnName(StoreObjectIdentifier.Table("auditoria_detalhe", null)));

            var auditoriaIdIndex = entity.FindIndex(auditoriaIdProperty);
            Assert.NotNull(auditoriaIdIndex);

            var nomePropriedadeIndex = entity.FindIndex(nomePropriedadeProperty);
            Assert.NotNull(nomePropriedadeIndex);
        }
    }
}
