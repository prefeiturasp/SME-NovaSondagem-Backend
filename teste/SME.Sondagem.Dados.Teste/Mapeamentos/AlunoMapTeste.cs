using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SME.Sondagem.Dados.Mapeamentos;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class AlunoMapTeste
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Aluno> Alunos { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseInMemoryDatabase("AlunoMapTest");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.ApplyConfiguration(new AlunoMap());
        }

        [Fact]
        public void Deve_Mapear_Propriedades_E_Indices_Corretamente()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Aluno));

            Assert.NotNull(entity);
            Assert.Equal("aluno", entity.GetTableName());
            Assert.Equal("pk_aluno", entity.FindPrimaryKey().GetName());
            Assert.NotNull(entity.FindProperty("RaAluno"));
            Assert.Equal("ra_aluno", entity.FindProperty("RaAluno").GetColumnName(StoreObjectIdentifier.Table("aluno", null)));
            Assert.NotNull(entity.FindIndex(entity.FindProperty("RaAluno")));
            Assert.True(entity.FindIndex(entity.FindProperty("RaAluno")).IsUnique);
        }
    }
}
