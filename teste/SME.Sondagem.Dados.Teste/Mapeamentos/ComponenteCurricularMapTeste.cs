using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SME.Sondagem.Dados.Mapeamentos;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class ComponenteCurricularMapTeste
    {
        private class TestDbContext : DbContext
        {
            public DbSet<ComponenteCurricular> ComponentesCurriculares { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseInMemoryDatabase("ComponenteCurricularMapTest");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.ApplyConfiguration(new ComponenteCurricularMap());
        }

        [Fact]
        public void Deve_Mapear_Propriedades_E_Indices_Corretamente()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(ComponenteCurricular));

            Assert.NotNull(entity);

            var primaryKey = entity.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Equal("pk_componente_curricular", primaryKey.GetName());

            var nomeProperty = entity.FindProperty("Nome");
            Assert.NotNull(nomeProperty);
            Assert.Equal("nome", nomeProperty.GetColumnName(StoreObjectIdentifier.Table("componente_curricular", null)));

            var codigoEolProperty = entity.FindProperty("CodigoEol");
            Assert.NotNull(codigoEolProperty);
            Assert.Equal("codigo_eol", codigoEolProperty.GetColumnName(StoreObjectIdentifier.Table("componente_curricular", null)));

            var index = entity.GetIndexes().FirstOrDefault(i =>
                i.Properties.Any(p => p.Name == "Nome") &&
                i.Properties.Any(p => p.Name == "Ano") &&
                i.Properties.Any(p => p.Name == "Modalidade"));
            Assert.NotNull(index);
            Assert.True(index.IsUnique);
            Assert.Equal("uk_componente_nome_ano_modalidade", index.GetDatabaseName());
        }
    }
}
