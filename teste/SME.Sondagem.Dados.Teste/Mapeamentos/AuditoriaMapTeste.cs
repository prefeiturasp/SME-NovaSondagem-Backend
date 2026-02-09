using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SME.Sondagem.Dados.Mapeamentos;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class AuditoriaMapTeste
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Auditoria> Auditorias { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseInMemoryDatabase("AuditoriaMapTest");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.ApplyConfiguration(new AuditoriaMap());
        }

        [Fact]
        public void Deve_Mapear_Propriedades_E_Indices_Corretamente()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Auditoria));

            Assert.NotNull(entity);

            var primaryKey = entity.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Equal("pk_auditoria", primaryKey.GetName());

            var idProperty = entity.FindProperty("Id");
            Assert.NotNull(idProperty);
            Assert.Equal("id", idProperty.GetColumnName(StoreObjectIdentifier.Table("auditoria", null)));

            var chaveProperty = entity.FindProperty("Chave");
            Assert.NotNull(chaveProperty);
            Assert.Equal("chave", chaveProperty.GetColumnName(StoreObjectIdentifier.Table("auditoria", null)));

            var acaoProperty = entity.FindProperty("Acao");
            Assert.NotNull(acaoProperty);
            Assert.Equal("acao", acaoProperty.GetColumnName(StoreObjectIdentifier.Table("auditoria", null)));

            var usuarioProperty = entity.FindProperty("Usuario");
            Assert.NotNull(usuarioProperty);
            Assert.Equal("usuario", usuarioProperty.GetColumnName(StoreObjectIdentifier.Table("auditoria", null)));
        }
    }
}
