using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SME.Sondagem.Dados.Mapeamentos;
using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;
using System.Linq;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class GrupoQuestoesMapTeste
    {
        private class TestDbContext : DbContext
        {
            public DbSet<GrupoQuestoes> GruposQuestoes { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseInMemoryDatabase("GrupoQuestoesMapTest");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.ApplyConfiguration(new GrupoQuestoesMap());
        }

        [Fact]
        public void Deve_Mapear_Propriedades_E_Relacionamentos_Corretamente()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(GrupoQuestoes));

            Assert.NotNull(entity);

            var primaryKey = entity.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Equal("pk_grupo_questoes", primaryKey.GetName());

            var tituloProperty = entity.FindProperty("Titulo");
            Assert.NotNull(tituloProperty);
            Assert.Equal("titulo", tituloProperty.GetColumnName(StoreObjectIdentifier.Table("grupo_questoes", null)));

            var subtituloProperty = entity.FindProperty("Subtitulo");
            Assert.NotNull(subtituloProperty);
            Assert.Equal("subtitulo", subtituloProperty.GetColumnName(StoreObjectIdentifier.Table("grupo_questoes", null)));

            var excluidoProperty = entity.FindProperty("Excluido");
            Assert.NotNull(excluidoProperty);
            Assert.Equal("excluido", excluidoProperty.GetColumnName(StoreObjectIdentifier.Table("grupo_questoes", null)));

            // Verifica relacionamento com Questoes
            var navigation = entity.GetNavigations().FirstOrDefault(n => n.Name == "Questoes");
            Assert.NotNull(navigation);
            Assert.Equal("Questoes", navigation.Name);
        }
    }
}
