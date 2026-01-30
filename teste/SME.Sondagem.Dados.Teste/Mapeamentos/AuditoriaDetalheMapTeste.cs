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
            Assert.Equal("auditoria_detalhe", entity.GetTableName());
            Assert.Equal("pk_auditoria_detalhe", entity.FindPrimaryKey().GetName());
            Assert.NotNull(entity.FindProperty("AuditoriaId"));
            Assert.Equal("auditoria_id", entity.FindProperty("AuditoriaId").GetColumnName(StoreObjectIdentifier.Table("auditoria_detalhe", null)));
            Assert.NotNull(entity.FindProperty("NomePropriedade"));
            Assert.Equal("nome_propriedade", entity.FindProperty("NomePropriedade").GetColumnName(StoreObjectIdentifier.Table("auditoria_detalhe", null)));
            Assert.NotNull(entity.FindIndex(entity.FindProperty("AuditoriaId")));
            Assert.NotNull(entity.FindIndex(entity.FindProperty("NomePropriedade")));
        }
    }
}
