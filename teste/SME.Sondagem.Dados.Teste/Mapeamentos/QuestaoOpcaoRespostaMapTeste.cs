using Microsoft.EntityFrameworkCore;
using Xunit;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class QuestaoOpcaoRespostaMapTeste
    {
        [Fact]
        public void Deve_Mapear_Propriedades_Indices_E_Relacionamentos_Corretamente()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new SondagemDbContext(options);

            var model = context.Model.FindEntityType(typeof(QuestaoOpcaoResposta));
            Assert.NotNull(model);

            // Propriedades principais
            Assert.NotNull(model.FindProperty("Id"));
            Assert.NotNull(model.FindProperty("QuestaoId"));
            Assert.NotNull(model.FindProperty("OpcaoRespostaId"));
            Assert.NotNull(model.FindProperty("Ordem"));
            Assert.NotNull(model.FindProperty("Excluido"));

            // Auditoria
            Assert.NotNull(model.FindProperty("CriadoEm"));
            Assert.NotNull(model.FindProperty("CriadoPor"));
            Assert.NotNull(model.FindProperty("AlteradoEm"));
            Assert.NotNull(model.FindProperty("AlteradoPor"));
            Assert.NotNull(model.FindProperty("CriadoRF"));
            Assert.NotNull(model.FindProperty("AlteradoRF"));

            // Índice único
            Assert.Contains(model.GetIndexes(), idx =>
                idx.Properties.Any(p => p.Name == "QuestaoId") &&
                idx.Properties.Any(p => p.Name == "OpcaoRespostaId") &&
                idx.IsUnique);

            // Relacionamentos
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Questao");
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "OpcaoResposta");
        }
    }
}
