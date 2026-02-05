using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SME.Sondagem.Dados.Mapeamentos;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class BimestreMapTeste
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Bimestre> Bimestres { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseInMemoryDatabase("BimestreMapTest");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.ApplyConfiguration(new BimestreMap());
        }

        [Fact]
        public void Deve_Mapear_Propriedades_E_Indices_Corretamente()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Bimestre));

            Assert.NotNull(entity);

            var primaryKey = entity.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Equal("pk_bimestre", primaryKey.GetName());

            var codBimestreProperty = entity.FindProperty("CodBimestreEnsinoEol");
            Assert.NotNull(codBimestreProperty);
            Assert.Equal("cod_bimestre_ensino_eol", codBimestreProperty.GetColumnName(StoreObjectIdentifier.Table("bimestre", null)));

            var descricaoProperty = entity.FindProperty("Descricao");
            Assert.NotNull(descricaoProperty);
            Assert.Equal("descricao", descricaoProperty.GetColumnName(StoreObjectIdentifier.Table("bimestre", null)));

            var codBimestreIndex = entity.FindIndex(codBimestreProperty);
            Assert.NotNull(codBimestreIndex);
            Assert.True(codBimestreIndex.IsUnique);

            var descricaoIndex = entity.FindIndex(descricaoProperty);
            Assert.NotNull(descricaoIndex);
            Assert.True(descricaoIndex.IsUnique);
        }
    }
}
