using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SME.Sondagem.Dados.Mapeamentos;
using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class OpcaoRespostaMapTeste
    {
        private class TestDbContext : DbContext
        {
            public DbSet<OpcaoResposta> OpcoesResposta { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseInMemoryDatabase("OpcaoRespostaMapTest");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.ApplyConfiguration(new OpcaoRespostaMap());
        }

        [Fact]
        public void Deve_Mapear_Propriedades_E_Relacionamentos_Corretamente()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(OpcaoResposta));

            Assert.NotNull(entity);

            var primaryKey = entity.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Equal("pk_opcao_resposta", primaryKey.GetName());

            var descricaoProperty = entity.FindProperty("DescricaoOpcaoResposta");
            Assert.NotNull(descricaoProperty);
            Assert.Equal("descricao_opcao_resposta", descricaoProperty.GetColumnName(StoreObjectIdentifier.Table("opcao_resposta", null)));

            var ordemProperty = entity.FindProperty("Ordem");
            Assert.NotNull(ordemProperty);
            Assert.Equal("ordem", ordemProperty.GetColumnName(StoreObjectIdentifier.Table("opcao_resposta", null)));

            var legendaProperty = entity.FindProperty("Legenda");
            Assert.NotNull(legendaProperty);
            Assert.Equal("legenda", legendaProperty.GetColumnName(StoreObjectIdentifier.Table("opcao_resposta", null)));

            var corFundoProperty = entity.FindProperty("CorFundo");
            Assert.NotNull(corFundoProperty);
            Assert.Equal("cor_fundo", corFundoProperty.GetColumnName(StoreObjectIdentifier.Table("opcao_resposta", null)));

            var corTextoProperty = entity.FindProperty("CorTexto");
            Assert.NotNull(corTextoProperty);
            Assert.Equal("cor_texto", corTextoProperty.GetColumnName(StoreObjectIdentifier.Table("opcao_resposta", null)));

            var excluidoProperty = entity.FindProperty("Excluido");
            Assert.NotNull(excluidoProperty);
            Assert.Equal("excluido", excluidoProperty.GetColumnName(StoreObjectIdentifier.Table("opcao_resposta", null)));

            var index = entity.GetIndexes().FirstOrDefault(i =>
                i.Properties.Any(p => p.Name == "DescricaoOpcaoResposta"));
            Assert.NotNull(index);
            Assert.True(index.IsUnique);
            Assert.Equal("uk_opcao_resposta_desc", index.GetDatabaseName());

            // Verifica relacionamento com QuestaoOpcoes
            var questaoOpcoesNav = entity.GetNavigations().FirstOrDefault(n => n.Name == "QuestaoOpcoes");
            Assert.NotNull(questaoOpcoesNav);

            // Verifica relacionamento com Respostas
            var respostasNav = entity.GetNavigations().FirstOrDefault(n => n.Name == "Respostas");
            Assert.NotNull(respostasNav);
        }
    }
}
