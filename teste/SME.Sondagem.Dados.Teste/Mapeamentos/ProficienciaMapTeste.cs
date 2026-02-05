using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SME.Sondagem.Dados.Mapeamentos;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class ProficienciaMapTeste
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Proficiencia> Proficiencias { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseInMemoryDatabase("ProficienciaMapTest");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.ApplyConfiguration(new ProficienciaMap());
        }

        [Fact]
        public void Deve_Mapear_Propriedades_E_Relacionamentos_Corretamente()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Proficiencia));

            Assert.NotNull(entity);

            var primaryKey = entity.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Equal("pk_proficiencia", primaryKey.GetName());

            var nomeProperty = entity.FindProperty("Nome");
            Assert.NotNull(nomeProperty);
            Assert.Equal("nome", nomeProperty.GetColumnName(StoreObjectIdentifier.Table("proficiencia", null)));

            var componenteCurricularIdProperty = entity.FindProperty("ComponenteCurricularId");
            Assert.NotNull(componenteCurricularIdProperty);
            Assert.Equal("componente_curricular_id", componenteCurricularIdProperty.GetColumnName(StoreObjectIdentifier.Table("proficiencia", null)));

            var excluidoProperty = entity.FindProperty("Excluido");
            Assert.NotNull(excluidoProperty);
            Assert.Equal("excluido", excluidoProperty.GetColumnName(StoreObjectIdentifier.Table("proficiencia", null)));

            var index = entity.GetIndexes().FirstOrDefault(i =>
                i.Properties.Any(p => p.Name == "Nome") &&
                i.Properties.Any(p => p.Name == "ComponenteCurricularId"));
            Assert.NotNull(index);
            Assert.True(index.IsUnique);
            Assert.Equal("uk_proficiencia_nome_componente", index.GetDatabaseName());

            // Verifica relacionamento com ComponenteCurricular
            var navigationComponente = entity.GetNavigations().FirstOrDefault(n => n.Name == "ComponenteCurricular");
            Assert.NotNull(navigationComponente);

            // Verifica relacionamento com Questionarios
            var navigationQuestionarios = entity.GetNavigations().FirstOrDefault(n => n.Name == "Questionarios");
            Assert.NotNull(navigationQuestionarios);
        }
    }
}
