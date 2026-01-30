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
            var primaryKey = entity.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Equal("pk_aluno", primaryKey.GetName());

            var raAlunoProperty = entity.FindProperty("RaAluno");
            Assert.NotNull(raAlunoProperty);

            var raAlunoIndex = entity.FindIndex(raAlunoProperty);
            Assert.NotNull(raAlunoIndex);
            Assert.True(raAlunoIndex.IsUnique);
        }
    }
}
