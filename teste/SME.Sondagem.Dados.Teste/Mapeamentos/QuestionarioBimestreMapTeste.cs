using Microsoft.EntityFrameworkCore;
using Xunit;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class QuestionarioBimestreMapTeste
    {
        [Fact]
        public void Deve_Mapear_Propriedades_Indices_E_Relacionamentos_Corretamente()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new SondagemDbContext(options);

            var model = context.Model.FindEntityType(typeof(QuestionarioBimestre));
            Assert.NotNull(model);

            // Propriedades principais
            Assert.NotNull(model.FindProperty("Id"));
            Assert.NotNull(model.FindProperty("QuestionarioId"));
            Assert.NotNull(model.FindProperty("BimestreId"));
            Assert.NotNull(model.FindProperty("Excluido"));

            // Auditoria
            Assert.NotNull(model.FindProperty("CriadoEm"));
            Assert.NotNull(model.FindProperty("CriadoPor"));
            Assert.NotNull(model.FindProperty("AlteradoEm"));
            Assert.NotNull(model.FindProperty("AlteradoPor"));
            Assert.NotNull(model.FindProperty("CriadoRF"));
            Assert.NotNull(model.FindProperty("AlteradoRF"));

            // Índices
            Assert.Contains(model.GetIndexes(), idx =>
                idx.Properties.Any(p => p.Name == "QuestionarioId") &&
                !idx.IsUnique);
            Assert.Contains(model.GetIndexes(), idx =>
                idx.Properties.Any(p => p.Name == "BimestreId") &&
                !idx.IsUnique);
            Assert.Contains(model.GetIndexes(), idx =>
                idx.Properties.Any(p => p.Name == "QuestionarioId") &&
                idx.Properties.Any(p => p.Name == "BimestreId") &&
                idx.IsUnique);

            // Relacionamentos
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Questionario");
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Bimestre");
        }
    }
}
